using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Columns;
using Simpled.Models;


using Simpled.Repository;

namespace Simpled.Services
{
    public class ColumnService : IColumnRepository
    {
        private readonly SimpledDbContext _context;

        public ColumnService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoardColumnReadDto>> GetAllAsync()
        {
            return await _context.BoardColumns
                .Select(c => new BoardColumnReadDto
                {
                    Id = c.Id,
                    BoardId = c.BoardId,
                    Title = c.Title,
                    Order = c.Order
                })
                .ToListAsync();
        }

        public async Task<BoardColumnReadDto?> GetByIdAsync(Guid id)
        {
            var column = await _context.BoardColumns.FindAsync(id);
            if (column == null) return null;

            return new BoardColumnReadDto
            {
                Id = column.Id,
                BoardId = column.BoardId,
                Title = column.Title,
                Order = column.Order
            };
        }

        public async Task<BoardColumnReadDto> CreateAsync(BoardColumnCreateDto dto)
        {
            var newColumn = new BoardColumn
            {
                Id = Guid.NewGuid(),
                BoardId = dto.BoardId,
                Title = dto.Title,
                Order = dto.Order
            };

            _context.BoardColumns.Add(newColumn);
            await _context.SaveChangesAsync();

            return new BoardColumnReadDto
            {
                Id = newColumn.Id,
                BoardId = newColumn.BoardId,
                Title = newColumn.Title,
                Order = newColumn.Order
            };
        }

        public async Task<bool> UpdateAsync(BoardColumnUpdateDto dto)
        {
            var column = await _context.BoardColumns.FindAsync(dto.Id);
            if (column == null) return false;

            column.BoardId = dto.BoardId;
            column.Title = dto.Title;
            column.Order = dto.Order;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var column = await _context.BoardColumns.FindAsync(id);
            if (column == null) return false;

            _context.BoardColumns.Remove(column);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
