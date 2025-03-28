using Simpled.Dtos.Boards;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simpled.Repository
{
    public interface IBoardRepository
    {
        Task<IEnumerable<BoardReadDto>> GetAllAsync();
        Task<BoardReadDto?> GetByIdAsync(Guid id);
        Task<BoardReadDto> CreateAsync(BoardCreateDto dto);
        Task<bool> UpdateAsync(BoardUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
