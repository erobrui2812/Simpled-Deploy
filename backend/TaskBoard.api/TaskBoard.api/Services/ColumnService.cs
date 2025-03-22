using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskBoard.api.Data;
using TaskBoard.api.Models;
using TaskBoard.api.Models.Dtos.ColumnDtos;

namespace TaskBoard.api.Services
{
    public class ColumnService : IColumnService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ColumnService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ColumnResponseDto>> GetColumnsByBoardAsync(Guid boardId)
        {
            var columns = await _context.Columns
                .AsNoTracking()
                .Where(c => c.BoardId == boardId)
                .ToListAsync();
            return _mapper.Map<List<ColumnResponseDto>>(columns);
        }

        public async Task<ColumnResponseDto> CreateColumnAsync(Guid boardId, ColumnCreateDto dto)
        {
            var column = new Column
            {
                BoardId = boardId,
                Title = dto.Title,
                Order = dto.Order
            };
            await _context.Columns.AddAsync(column);
            await _context.SaveChangesAsync();
            return _mapper.Map<ColumnResponseDto>(column);
        }

        public async Task<ColumnResponseDto> GetColumnAsync(Guid boardId, Guid columnId)
        {
            var column = await _context.Columns
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
            if (column == null) throw new KeyNotFoundException("Column not found");
            return _mapper.Map<ColumnResponseDto>(column);
        }

        public async Task<ColumnResponseDto> UpdateColumnAsync(Guid boardId, Guid columnId, ColumnUpdateDto dto)
        {
            var column = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
            if (column == null) throw new KeyNotFoundException("Column not found");
            column.Title = dto.Title;
            column.Order = dto.Order;
            await _context.SaveChangesAsync();
            return _mapper.Map<ColumnResponseDto>(column);
        }

        public async Task DeleteColumnAsync(Guid boardId, Guid columnId)
        {
            var column = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
            if (column == null) throw new KeyNotFoundException("Column not found");
            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();
        }
    }
}
