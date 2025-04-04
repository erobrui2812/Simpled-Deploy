using Simpled.Dtos.Boards;

namespace Simpled.Repository
{
    public interface IBoardRepository
    {
        Task<IEnumerable<BoardReadDto>> GetAllAsync(Guid? userId = null);
        Task<BoardReadDto?> GetByIdAsync(Guid id);
        Task<BoardReadDto> CreateAsync(BoardCreateDto dto, Guid userId);
        Task<bool> UpdateAsync(BoardUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
