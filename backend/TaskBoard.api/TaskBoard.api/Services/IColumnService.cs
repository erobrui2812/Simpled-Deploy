using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskBoard.api.Models.Dtos.ColumnDtos;

namespace TaskBoard.api.Services
{
    public interface IColumnService
    {
        Task<List<ColumnResponseDto>> GetColumnsByBoardAsync(Guid boardId);
        Task<ColumnResponseDto> CreateColumnAsync(Guid boardId, ColumnCreateDto dto);
        Task<ColumnResponseDto> GetColumnAsync(Guid boardId, Guid columnId);
        Task<ColumnResponseDto> UpdateColumnAsync(Guid boardId, Guid columnId, ColumnUpdateDto dto);
        Task DeleteColumnAsync(Guid boardId, Guid columnId);
    }
}
