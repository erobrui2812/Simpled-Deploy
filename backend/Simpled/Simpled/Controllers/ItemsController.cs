using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Crea una nueva instancia de <see cref="ItemsController"/>.
        /// </summary>
        /// <param name="itemService">Servicio/repo de ítems.</param>
        /// <param name="memberRepo">Repositorio de miembros de tablero.</param>
        public ItemsController(IItemRepository itemService, IBoardMemberRepository memberRepo)
        {
            _itemService = itemService;
            _memberRepo = memberRepo;
        }

        /// <summary>
        /// Obtiene todos los ítems, incluyendo sus subtareas y progreso.
        /// </summary>
        /// <returns>Lista de <see cref="ItemReadDto"/>.</returns>
        /// <response code="200">Ítems recuperados correctamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemReadDto>), 200)]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Obtiene un ítem por su identificador, incluyendo subtareas y progreso.
        /// </summary>
        /// <param name="id">Identificador del ítem.</param>
        /// <returns>Detalle del ítem.</returns>
        /// <response code="200">Ítem encontrado.</response>
        /// <response code="404">Ítem no encontrado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ItemReadDto), 200)]
        [ProducesResponseType(404)]
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
        /// Crea un nuevo ítem en una columna específica.
        /// </summary>
        /// <param name="dto">Datos para la creación del ítem.</param>
        /// <returns>Ítem creado.</returns>
        /// <response code="201">Ítem creado correctamente.</response>
        /// <response code="400">Datos de entrada no válidos.</response>
        /// <response code="403">Sin permisos para crear en este tablero.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ItemReadDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
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

        /// <summary>
        /// Actualiza un ítem existente. Los administradores pueden modificar todo, los editores solo el estado de sus propias tareas.
        /// </summary>
        /// <param name="id">Identificador del ítem.</param>
        /// <param name="dto">Datos actualizados del ítem.</param>
        /// <response code="204">Ítem actualizado correctamente.</response>
        /// <response code="400">ID en ruta y DTO no coinciden.</response>
        /// <response code="403">Sin permisos para modificar este ítem.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
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
        /// Elimina un ítem por su identificador (solo rol admin).
        /// </summary>
        /// <param name="id">Identificador del ítem.</param>
        /// <response code="204">Ítem eliminado correctamente.</response>
        /// <response code="403">Sin permisos para eliminar este ítem.</response>
        /// <response code="404">Ítem no encontrado.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Sube un archivo (imagen) y lo asocia al ítem.
        /// </summary>
        /// <param name="id">Identificador del ítem.</param>
        /// <param name="file">Archivo a subir.</param>
        /// <returns>Contenido subido.</returns>
        /// <response code="200">Archivo subido correctamente.</response>
        /// <response code="400">Archivo inválido o ítem no encontrado.</response>
        /// <response code="403">Sin permisos para subir archivos.</response>
        [HttpPost("{id:guid}/upload")]
        [ProducesResponseType(typeof(Content), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
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
