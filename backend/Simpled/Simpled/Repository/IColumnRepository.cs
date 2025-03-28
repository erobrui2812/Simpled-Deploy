using Simpled.Dtos.Columns;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simpled.Repository
{
    public interface IColumnRepository
    {
        Task<IEnumerable<BoardColumnReadDto>> GetAllAsync();
        Task<BoardColumnReadDto?> GetByIdAsync(Guid id);
        Task<BoardColumnReadDto> CreateAsync(BoardColumnCreateDto dto);
        Task<bool> UpdateAsync(BoardColumnUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
