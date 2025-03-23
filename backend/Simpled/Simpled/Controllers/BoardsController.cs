using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Boards;
using Simpled.Models.TrelloNotionClone.Models;
using Microsoft.AspNetCore.Authorization;
using Simpled.Models;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController : ControllerBase
    {
        private readonly SimpledDbContext _context;

        public BoardsController(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve todos los tableros existentes.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardReadDto>>> GetBoards()
        {
            var boards = await _context.Boards.ToListAsync();

            var boardDtos = boards.Select(b => new BoardReadDto
            {
                Id = b.Id,
                Name = b.Name,
                OwnerId = b.OwnerId,
                IsPublic = b.IsPublic
            }).ToList();

            return Ok(boardDtos);
        }

        /// <summary>
        /// Devuelve los detalles de un tablero por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardReadDto>> GetBoard(Guid id)
        {
            var board = await _context.Boards.FindAsync(id);
            if (board == null)
                return NotFound("Board not found.");

            var boardDto = new BoardReadDto
            {
                Id = board.Id,
                Name = board.Name,
                OwnerId = board.OwnerId,
                IsPublic = board.IsPublic
            };

            return Ok(boardDto);
        }

        /// <summary>
        /// Crea un nuevo tablero.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BoardReadDto>> CreateBoard([FromBody] BoardCreateDto createDto)
        {
            var newBoard = new Board
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                OwnerId = createDto.OwnerId,
                IsPublic = createDto.IsPublic
            };

            _context.Boards.Add(newBoard);
            await _context.SaveChangesAsync();

            var readDto = new BoardReadDto
            {
                Id = newBoard.Id,
                Name = newBoard.Name,
                OwnerId = newBoard.OwnerId,
                IsPublic = newBoard.IsPublic
            };

            return CreatedAtAction(nameof(GetBoard), new { id = newBoard.Id }, readDto);
        }

        /// <summary>
        /// Actualiza los detalles de un tablero.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] BoardUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID mismatch.");

            var existing = await _context.Boards.FindAsync(id);
            if (existing == null)
                return NotFound("Board not found.");

            existing.Name = updateDto.Name;
            existing.OwnerId = updateDto.OwnerId;
            existing.IsPublic = updateDto.IsPublic;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Elimina un tablero existente.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBoard(Guid id)
        {
            var board = await _context.Boards.FindAsync(id);
            if (board == null)
                return NotFound("Board not found.");

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}



