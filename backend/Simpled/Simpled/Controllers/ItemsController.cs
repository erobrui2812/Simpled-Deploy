
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Simpled.Dtos.Items;
using Simpled.Exception;
using Simpled.Helpers;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemService;
        private readonly IBoardMemberRepository _memberRepo;

        public ItemsController(IItemRepository itemService, IBoardMemberRepository memberRepo)
        {
            _itemService = itemService;
            _memberRepo = memberRepo;
        }

        /// <summary>
        /// Lista todos los ítems.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
            => Ok(await _itemService.GetAllAsync());

        /// <summary>
        /// Obtiene un ítem específico por su ID.
        /// </summary>
        [HttpGet("{id}")]
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

        /// <summary>
        /// Crea un nuevo ítem dentro de una columna.
        /// Solo admin/editor pueden crear; solo admin puede asignar responsables.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateDto dto)
        {
            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
                return Forbid("No tienes permisos para crear ítems en este tablero.");

            if (dto.AssigneeId.HasValue
                && !await BoardAuthorizationHelper.HasBoardPermissionAsync(
                        User, boardId, new[] { "admin" }, _memberRepo))
                return Forbid("Solo el admin puede asignar responsables.");

            var created = await _itemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetItem), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un ítem existente.
        /// Admin puede editar todo; editor asignado solo estado.
        /// </summary>
        [HttpPut("{id}")]
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

        /// <summary>
        /// Elimina un ítem existente (solo rol admin).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin" }, _memberRepo))
                return Forbid("No tienes permisos para eliminar este ítem.");

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

        /// <summary>
        /// Sube un archivo (imagen) a un ítem.
        /// </summary>
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
                return Forbid("No tienes permisos para subir archivos en este ítem.");

            var content = await _itemService.UploadFileAsync(id, file);
            return content == null
                ? BadRequest("Error al subir archivo o ítem no encontrado.")
                : Ok(content);
        }
    }
}
