using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Boards;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardRepository _boardService;

        public BoardsController(IBoardRepository boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBoards()
        {
            var result = await _boardService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBoard(Guid id)
        {
            var board = await _boardService.GetByIdAsync(id);
            return board == null ? NotFound("Board not found.") : Ok(board);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard([FromBody] BoardCreateDto dto)
        {
            var created = await _boardService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBoard), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] BoardUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var success = await _boardService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Board not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(Guid id)
        {
            var success = await _boardService.DeleteAsync(id);
            return success ? NoContent() : NotFound("Board not found.");
        }
    }
}
