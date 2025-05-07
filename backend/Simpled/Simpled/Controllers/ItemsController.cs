using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Items;
using Simpled.Exception;
using Simpled.Helpers;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Controllers
{
    /// <summary>
    /// Gestiona las operaciones CRUD de ítems en un tablero Kanban, incluyendo subida de archivos.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemService;
        private readonly IBoardMemberRepository _memberRepo;

        public ItemsController(
            IItemRepository itemService,
            IBoardMemberRepository memberRepo)
        {
            _itemService = itemService;
            _memberRepo = memberRepo;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ItemReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            try
            {
                var item = await _itemService.GetByIdAsync(id);
                return Ok(item);
            }
            catch (NotFoundException)
            {
                return NotFound("Ítem no encontrado.");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateDto dto)
        {
            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
            {
                return Forbid("No tienes permisos para crear ítems en este tablero.");
            }

            if (dto.AssigneeId.HasValue
                && !await BoardAuthorizationHelper.HasBoardPermissionAsync(
                        User, boardId, new[] { "admin" }, _memberRepo))
            {
                return Forbid("Solo el admin puede asignar responsables.");
            }

            var created = await _itemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetItem), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var member = await _memberRepo.GetByIdsAsync(boardId, userId);

            if (member == null)
                return Forbid("No eres miembro de este tablero.");

            if (member.Role == "admin")
            {
                await _itemService.UpdateAsync(dto);
                return NoContent();
            }
            else if (member.Role == "editor" && dto.AssigneeId == userId)
            {
                await _itemService.UpdateStatusAsync(id, dto.Status);
                return NoContent();
            }
            else
            {
                return Forbid("No tienes permisos para modificar este ítem.");
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin" }, _memberRepo))
            {
                return Forbid("No tienes permisos para eliminar este ítem.");
            }

            try
            {
                await _itemService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound("Ítem no encontrado.");
            }
        }

        [HttpPost("{id:guid}/upload")]
        [ProducesResponseType(typeof(Content), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
            {
                return Forbid("No tienes permisos para subir archivos en este ítem.");
            }

            var content = await _itemService.UploadFileAsync(id, file);
            return content == null
                ? BadRequest("Error al subir archivo o ítem no encontrado.")
                : Ok(content);
        }
    }
}
