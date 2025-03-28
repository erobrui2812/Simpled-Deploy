using Simpled.Dtos.BoardMembers;

namespace Simpled.Repository
{
    public interface IBoardMemberRepository
    {
        Task<IEnumerable<BoardMemberReadDto>> GetAllAsync();
        Task<IEnumerable<BoardMemberReadDto>> GetByBoardIdAsync(Guid boardId);
        Task<BoardMemberReadDto?> GetByIdsAsync(Guid boardId, Guid userId);
        Task AddAsync(BoardMemberCreateDto dto);
        Task AddManyAsync(List<BoardMemberCreateDto> dtos);
        Task<bool> UpdateAsync(BoardMemberUpdateDto dto);
        Task<bool> DeleteAsync(Guid boardId, Guid userId);
    }
}
