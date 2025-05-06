using Simpled.Dtos.Boards;
using Simpled.Models;

namespace Simpled.Repository
{
    public interface IBoardRepository
    {
        Task<IEnumerable<BoardReadDto>> GetAllAsync(Guid? userId = null);
        Task<BoardReadDto?> GetByIdAsync(Guid id, Guid userId);
        Task<BoardReadDto> CreateAsync(BoardCreateDto dto, Guid userId);
        Task<bool> UpdateAsync(BoardUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<Board?> GetBoardByIdAsync(Guid id);
    }
}
