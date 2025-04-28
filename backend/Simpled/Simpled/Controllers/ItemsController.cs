
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IBoardMemberRepository _boardMemberRepo;

        public ItemsController(IItemRepository itemService, IBoardMemberRepository boardMemberRepo)
        {
            _itemService = itemService;
            _boardMemberRepo = boardMemberRepo;
        }

        /// <summary>
        /// Lista todos los ítems
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var result = await _itemService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un ítem específico.
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
                return NotFound("Item no encontrado.");
            }
        }

        /// <summary>
        /// Crea un nuevo ítem dentro de una columna.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateDto dto)
        {
            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _boardMemberRepo))
            {
                return Forbid("No tienes permisos suficientes para crear un item en este board.");
            }

            var created = await _itemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetItem), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza los datos de un ítem.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _boardMemberRepo))
            {
                return Forbid("No tienes permisos suficientes para editar este item.");
            }

            try
            {
                await _itemService.UpdateAsync(dto);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound("Item no encontrado.");
            }
        }

        /// <summary>
        /// Elimina un ítem existente (requiere rol admin).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin" }, _boardMemberRepo))
            {
                return Forbid("No tienes permisos suficientes para eliminar este item.");
            }

            try
            {
                await _itemService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound("Item no encontrado.");
            }
        }

        /// <summary>
        /// Sube un archivo (imagen) a un ítem determinado.
        /// </summary>
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _boardMemberRepo))
            {
                return Forbid("No tienes permisos suficientes para subir archivos en este item.");
            }

            var content = await _itemService.UploadFileAsync(id, file);
            return content == null
                ? BadRequest("Error al subir archivo o ítem no encontrado.")
                : Ok(content);
        }
    }
}
