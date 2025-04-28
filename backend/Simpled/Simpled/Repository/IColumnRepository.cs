
using Simpled.Dtos.Columns;


namespace Simpled.Repository
{
    public interface IColumnRepository
    {
        Task<IEnumerable<BoardColumnReadDto>> GetAllAsync();
        Task<BoardColumnReadDto?> GetByIdAsync(Guid id);
        Task<BoardColumnReadDto> CreateAsync(BoardColumnCreateDto dto);
        Task<bool> UpdateAsync(BoardColumnUpdateDto dto);
       
        Task<bool> DeleteAsync(Guid columnId, bool cascadeItems = false, Guid? targetColumnId = null);

        Task<Guid> GetBoardIdByColumnId(Guid columnId);
    }
}
