using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Subtasks;
using Simpled.Helpers;
using Simpled.Repository;

namespace Simpled.Controllers
{
    /// <summary>
    /// Gestiona las subtareas (checklist) de un ítem en el tablero.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/items/{itemId:guid}/subtasks")]
    public class SubtasksController : ControllerBase
    {
        private readonly IItemRepository _itemService;
        private readonly IBoardMemberRepository _memberRepo;

        /// <summary>
        /// Crea una instancia de <see cref="SubtasksController"/>.
        /// </summary>
        /// <param name="itemService">Repositorio de ítems (incluye subtareas).</param>
        /// <param name="memberRepo">Repositorio de miembros del tablero.</param>
        public SubtasksController(IItemRepository itemService, IBoardMemberRepository memberRepo)
        {
            _itemService = itemService;
            _memberRepo = memberRepo;
        }

        /// <summary>
        /// Obtiene todas las subtareas asociadas a un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <returns>Listado de subtareas.</returns>
        /// <response code="200">Devuelve la lista de subtareas.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SubtaskDto>), 200)]
        public async Task<IActionResult> GetAll(Guid itemId)
        {
            var subtasks = await _itemService.GetSubtasksByItemIdAsync(itemId);
            return Ok(subtasks);
        }

        /// <summary>
        /// Crea una nueva subtarea para un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <param name="dto">Datos de la subtarea a crear.</param>
        /// <returns>Subtarea creada.</returns>
        /// <response code="201">Subtarea creada correctamente.</response>
        /// <response code="400">El ItemId del DTO no coincide con la ruta.</response>
        /// <response code="403">El usuario no tiene permisos para crear subtareas.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SubtaskDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create(Guid itemId, [FromBody] SubtaskCreateDto dto)
        {
            if (dto.ItemId != itemId)
                return BadRequest("El ItemId de la carga no coincide con la ruta.");

            var boardId = await _itemService.GetBoardIdByItemId(itemId);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
                return Forbid("No tienes permisos para crear subtareas.");

            var created = await _itemService.CreateSubtaskAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { itemId }, created);
        }

        /// <summary>
        /// Actualiza una subtarea existente.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <param name="id">Identificador de la subtarea.</param>
        /// <param name="dto">Datos de la subtarea a actualizar.</param>
        /// <returns>Sin contenido.</returns>
        /// <response code="204">Subtarea actualizada correctamente.</response>
        /// <response code="400">ID de subtarea o ItemId incorrectos.</response>
        /// <response code="403">El usuario no tiene permisos para editar subtareas.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Update(Guid itemId, Guid id, [FromBody] SubtaskUpdateDto dto)
        {
            if (dto.ItemId != itemId || dto.Id != id)
                return BadRequest("ID de subtarea o ItemId incorrectos.");

            var boardId = await _itemService.GetBoardIdByItemId(itemId);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
                return Forbid("No tienes permisos para editar subtareas.");

            await _itemService.UpdateSubtaskAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// Elimina una subtarea de un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <param name="id">Identificador de la subtarea.</param>
        /// <returns>Sin contenido.</returns>
        /// <response code="204">Subtarea eliminada correctamente.</response>
        /// <response code="403">El usuario no tiene permisos para eliminar subtareas.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Delete(Guid itemId, Guid id)
        {
            var boardId = await _itemService.GetBoardIdByItemId(itemId);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
                return Forbid("No tienes permisos para eliminar subtareas.");

            await _itemService.DeleteSubtaskAsync(id);
            return NoContent();
        }
    }
}
