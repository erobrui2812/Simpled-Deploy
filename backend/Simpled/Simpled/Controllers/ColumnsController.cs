using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Columns;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnRepository _columnService;

        public ColumnsController(IColumnRepository columnService)
        {
            _columnService = columnService;
        }

        /// <summary>
        /// Lista todas las columnas de todos los tableros.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllColumns()
        {
            var result = await _columnService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una columna por ID.
        /// </summary>
        /// <param name="id">ID de la columna</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetColumn(Guid id)
        {
            var column = await _columnService.GetByIdAsync(id);
            return column == null ? NotFound("Column not found.") : Ok(column);
        }

        /// <summary>
        /// Crea una nueva columna en un tablero.
        /// </summary>
        /// <param name="dto">Datos de la columna a crear</param>
        [HttpPost]
        public async Task<IActionResult> CreateColumn([FromBody] BoardColumnCreateDto dto)
        {
            var created = await _columnService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetColumn), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza una columna existente.
        /// </summary>
        /// <param name="id">ID de la columna</param>
        /// <param name="dto">Datos de la columna a actualizar</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColumn(Guid id, [FromBody] BoardColumnUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var success = await _columnService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Column not found.");
        }

        /// <summary>
        /// Elimina una columna (requiere rol admin).
        /// </summary>
        /// <param name="id">ID de la columna</param>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(Guid id)
        {
            var success = await _columnService.DeleteAsync(id);
            return success ? NoContent() : NotFound("Column not found.");
        }
    }
}

