using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Columns;
using Simpled.Helpers;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnRepository _columnService;
        private readonly IBoardMemberRepository _boardMemberRepo;

        public ColumnsController(IColumnRepository columnService, IBoardMemberRepository boardMemberRepo)
        {
            _columnService = columnService;
            _boardMemberRepo = boardMemberRepo;
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
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, dto.BoardId, new[] { "admin", "editor" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos para crear columnas en este tablero.");

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

            var boardId = await _columnService.GetBoardIdByColumnId(dto.Id);
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin", "editor" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos para modificar columnas en este tablero.");

            var success = await _columnService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Column not found.");
        }

        /// <summary>
        /// Elimina una columna (requiere rol admin en el board).
        /// </summary>
        /// <param name="id">ID de la columna</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(Guid id)
        {
            var boardId = await _columnService.GetBoardIdByColumnId(id);
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos para eliminar columnas en este tablero.");

            var success = await _columnService.DeleteAsync(id);
            return success ? NoContent() : NotFound("Column not found.");
        }
    }
}
