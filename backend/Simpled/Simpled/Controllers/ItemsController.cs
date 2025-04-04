using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Items;
using Simpled.Helpers;
using Simpled.Repository;
using System.Security.Claims;

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
        /// Lista todos los ítems (tarjetas).
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
        /// <param name="id">ID del ítem</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var item = await _itemService.GetByIdAsync(id);
            return item == null ? NotFound("Item not found.") : Ok(item);
        }

        /// <summary>
        /// Crea un nuevo ítem dentro de una columna.
        /// </summary>
        /// <param name="dto">Datos del ítem a crear</param>
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateDto dto)
        {
            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin", "editor" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos suficientes para crear un item en este board.");

            var created = await _itemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetItem), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza los datos de un ítem.
        /// </summary>
        /// <param name="id">ID del ítem</param>
        /// <param name="dto">Datos del ítem a actualizar</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin", "editor" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos suficientes para editar este item.");

            var success = await _itemService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Item not found.");
        }

        /// <summary>
        /// Elimina un ítem existente (requiere rol admin).
        /// </summary>
        /// <param name="id">ID del ítem</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos suficientes para eliminar este item.");

            var success = await _itemService.DeleteAsync(id);
            return success ? NoContent() : NotFound("Item not found.");
        }

        /// <summary>
        /// Sube un archivo (imagen) a un ítem determinado.
        /// </summary>
        /// <param name="id">ID del ítem</param>
        /// <param name="file">Archivo a subir</param>
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin", "editor" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos suficientes para subir archivos en este item.");

            var result = await _itemService.UploadFileAsync(id, file);
            return result == null ? BadRequest("Error uploading file or item not found.") : Ok(result);
        }
    }
}
