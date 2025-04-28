// ColumnsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Columns;
using Simpled.Exception;
using Simpled.Helpers;
using Simpled.Repository;
using System;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<IActionResult> GetAllColumns()
            => Ok(await _columnService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColumn(Guid id)
        {
            try
            {
                var col = await _columnService.GetByIdAsync(id);
                return Ok(col);
            }
            catch (NotFoundException)
            {
                return NotFound("Columna no encontrada.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateColumn([FromBody] BoardColumnCreateDto dto)
        {
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, dto.BoardId, new[] { "admin", "editor" }, _boardMemberRepo))
                return Forbid("No tienes permisos para crear columnas en este tablero.");

            var created = await _columnService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetColumn), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColumn(Guid id, [FromBody] BoardColumnUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var boardId = await _columnService.GetBoardIdByColumnId(id);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _boardMemberRepo))
                return Forbid("No tienes permisos para modificar columnas en este tablero.");

            try
            {
                await _columnService.UpdateAsync(dto);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound("Columna no encontrada.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(
            Guid id,
            [FromQuery] bool cascadeItems = false,
            [FromQuery] Guid? targetColumnId = null)
        {
            var boardId = await _columnService.GetBoardIdByColumnId(id);
            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin" }, _boardMemberRepo))
                return Forbid("No tienes permisos para eliminar columnas en este tablero.");

            try
            {
                await _columnService.DeleteAsync(id, cascadeItems, targetColumnId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException)
            {
                return NotFound("Columna no encontrada.");
            }
        }
    }
}
