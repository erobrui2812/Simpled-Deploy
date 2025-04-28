
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Columns;
using Simpled.Exception;
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
            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

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
            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            column.BoardId = dto.BoardId;
            column.Title = dto.Title;
            column.Order = dto.Order;

            await _context.SaveChangesAsync();
            return true;
        }


        public Task<bool> DeleteAsync(Guid id)
        {
            return DeleteAsync(id, cascadeItems: false, targetColumnId: null);
        }

        public async Task<bool> DeleteAsync(Guid columnId, bool cascadeItems = false, Guid? targetColumnId = null)
        {
            var column = await _context.BoardColumns
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == columnId);

            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            if (column.Items.Any())
            {
                if (targetColumnId.HasValue)
                {
                    var target = await _context.BoardColumns.FindAsync(targetColumnId.Value);
                    if (target == null)
                        throw new NotFoundException("Columna destino no encontrada.");

                    foreach (var item in column.Items)
                        item.ColumnId = targetColumnId.Value;
                }
                else if (cascadeItems)
                {
                    _context.Items.RemoveRange(column.Items);
                }
                else
                {
                    throw new InvalidOperationException(
                        "La columna contiene tareas. Debes moverlas o usar cascadeItems=true.");
                }
            }

            _context.BoardColumns.Remove(column);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Guid> GetBoardIdByColumnId(Guid columnId)
        {
            var column = await _context.BoardColumns.FindAsync(columnId);
            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            return column.BoardId;
        }
    }
}
