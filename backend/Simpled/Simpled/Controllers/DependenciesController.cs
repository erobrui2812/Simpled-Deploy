using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Dependencies;
using Simpled.Models;
using Simpled.Services;

namespace Simpled.Controllers
{
    /// <summary>
    /// Gestiona las dependencias entre tareas en el diagrama de Gantt.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DependenciesController : ControllerBase
    {
        private readonly DependencyService _dependencyService;

        
        public DependenciesController(DependencyService dependencyService)
        {
            _dependencyService = dependencyService;
        }

        /// <summary>
        /// Obtiene todas las dependencias para un tablero.
        /// </summary>
        /// <param name="boardId">Identificador del tablero.</param>
        /// <response code="200">Listado de dependencias.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DependencyReadDto>), 200)]
        public async Task<IActionResult> Get([FromQuery] Guid boardId)
        {
    
            var deps = await _dependencyService.GetByBoardAsync(boardId);
            var dtos = deps.Select(d => new DependencyReadDto
            {
                Id = d.Id,
                FromTaskId = d.FromTaskId,
                ToTaskId = d.ToTaskId,
                Type = d.Type
            });
            return Ok(dtos);
        }

        /// <summary>
        /// Crea una nueva dependencia entre tareas.
        /// </summary>
        /// <param name="dto">Datos de la nueva dependencia.</param>
        /// <response code="201">Dependencia creada.</response>
        /// <response code="400">DTO inválido.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DependencyReadDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] DependencyCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var model = new Dependency
            {
                FromTaskId = dto.FromTaskId,
                ToTaskId = dto.ToTaskId,
                Type = dto.Type,
                BoardId = dto.BoardId
            };

            var created = await _dependencyService.CreateAsync(model);

            var readDto = new DependencyReadDto
            {
                Id = created.Id,
                FromTaskId = created.FromTaskId,
                ToTaskId = created.ToTaskId,
                Type = created.Type
            };

            return CreatedAtAction(nameof(Get), new { boardId = dto.BoardId }, readDto);
        }

        /// <summary>
        /// Elimina una dependencia existente.
        /// </summary>
        /// <param name="id">Identificador de la dependencia.</param>
        /// <response code="204">Eliminación exitosa.</response>
        /// <response code="404">Dependencia no encontrada.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _dependencyService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Dependencia con id {id} no encontrada.");
            }
        }
    }
}
