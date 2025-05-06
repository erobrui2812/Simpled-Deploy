using Simpled.Dtos.Items;
using Simpled.Dtos.Subtasks;
using Simpled.Models;

namespace Simpled.Repository
{
    public interface IItemRepository
    {
        Task<IEnumerable<ItemReadDto>> GetAllAsync();
        Task<ItemReadDto?> GetByIdAsync(Guid id);
        Task<ItemReadDto> CreateAsync(ItemCreateDto dto);
        Task<bool> UpdateAsync(ItemUpdateDto dto);
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> DeleteAsync(Guid id);
        Task<Content?> UploadFileAsync(Guid itemId, IFormFile file);
        Task<Guid> GetBoardIdByColumnId(Guid columnId);
        Task<Guid> GetBoardIdByItemId(Guid itemId);
        Task<IEnumerable<SubtaskDto>> GetSubtasksByItemIdAsync(Guid itemId);
        Task<SubtaskDto> CreateSubtaskAsync(SubtaskCreateDto dto);
        Task<bool> UpdateSubtaskAsync(SubtaskUpdateDto dto);
        Task<bool> DeleteSubtaskAsync(Guid subtaskId);
    }
}
