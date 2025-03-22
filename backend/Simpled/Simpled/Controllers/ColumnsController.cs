using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Models.TrelloNotionClone.Models;
using Simpled.Dtos.Columns;
using Microsoft.AspNetCore.Authorization;

namespace Simpled.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnsController : ControllerBase
    {
        private readonly SimpledDbContext _context;

        public ColumnsController(SimpledDbContext context)
        {
            _context = context;
        }

        // GET /api/columns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardColumnReadDto>>> GetAllColumns()
        {
            var columns = await _context.BoardColumns.ToListAsync();
            var columnDtos = columns.Select(c => new BoardColumnReadDto
            {
                Id = c.Id,
                BoardId = c.BoardId,
                Title = c.Title,
                Order = c.Order
            }).ToList();

            return Ok(columnDtos);
        }

        // GET /api/columns/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardColumnReadDto>> GetColumn(Guid id)
        {
            var column = await _context.BoardColumns.FindAsync(id);
            if (column == null)
                return NotFound("Column not found.");

            var columnDto = new BoardColumnReadDto
            {
                Id = column.Id,
                BoardId = column.BoardId,
                Title = column.Title,
                Order = column.Order
            };

            return Ok(columnDto);
        }

        // POST /api/columns
        [HttpPost]
        public async Task<ActionResult<BoardColumnReadDto>> CreateColumn([FromBody] BoardColumnCreateDto createDto)
        {
            var newColumn = new BoardColumn
            {
                Id = Guid.NewGuid(),
                BoardId = createDto.BoardId,
                Title = createDto.Title,
                Order = createDto.Order
            };

            _context.BoardColumns.Add(newColumn);
            await _context.SaveChangesAsync();

            var columnRead = new BoardColumnReadDto
            {
                Id = newColumn.Id,
                BoardId = newColumn.BoardId,
                Title = newColumn.Title,
                Order = newColumn.Order
            };

            return CreatedAtAction(nameof(GetColumn), new { id = newColumn.Id }, columnRead);
        }

        // PUT /api/columns/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColumn(Guid id, [FromBody] BoardColumnUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID mismatch.");

            var existing = await _context.BoardColumns.FindAsync(id);
            if (existing == null)
                return NotFound("Column not found.");

            existing.BoardId = updateDto.BoardId;
            existing.Title = updateDto.Title;
            existing.Order = updateDto.Order;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/columns/{id}
        // Solo admin puede borrar columnas
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(Guid id)
        {
            var column = await _context.BoardColumns.FindAsync(id);
            if (column == null)
                return NotFound("Column not found.");

            _context.BoardColumns.Remove(column);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
