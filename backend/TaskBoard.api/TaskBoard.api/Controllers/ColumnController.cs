using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.api.Services;
using TaskBoard.api.Filters;
using TaskBoard.api.Models.Dtos.ColumnDtos;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/boards/{boardId:guid}/columns")]
    [Authorize]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _columnService;

        public ColumnController(IColumnService columnService)
        {
            _columnService = columnService;
        }

        [HttpGet]
        [ServiceFilter(typeof(BoardAccessFilter))]
        public async Task<IActionResult> GetColumns(Guid boardId)
        {
            var columns = await _columnService.GetColumnsByBoardAsync(boardId);
            return Ok(columns);
        }

        [HttpPost]
        [ServiceFilter(typeof(BoardAccessFilter))]
        public async Task<IActionResult> CreateColumn(Guid boardId, [FromBody] ColumnCreateDto dto)
        {
            var column = await _columnService.CreateColumnAsync(boardId, dto);
            return CreatedAtAction(nameof(GetColumn), new { boardId, columnId = column.Id }, column);
        }

        [HttpGet("{columnId:guid}")]
        [ServiceFilter(typeof(BoardAccessFilter))]
        public async Task<IActionResult> GetColumn(Guid boardId, Guid columnId)
        {
            var column = await _columnService.GetColumnAsync(boardId, columnId);
            return Ok(column);
        }

        [HttpPut("{columnId:guid}")]
        [ServiceFilter(typeof(BoardAccessFilter))]
        public async Task<IActionResult> UpdateColumn(Guid boardId, Guid columnId, [FromBody] ColumnUpdateDto dto)
        {
            var column = await _columnService.UpdateColumnAsync(boardId, columnId, dto);
            return Ok(column);
        }

        [HttpDelete("{columnId:guid}")]
        [ServiceFilter(typeof(BoardAccessFilter))]
        public async Task<IActionResult> DeleteColumn(Guid boardId, Guid columnId)
        {
            await _columnService.DeleteColumnAsync(boardId, columnId);
            return NoContent();
        }
    }
}
