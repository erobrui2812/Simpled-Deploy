using Simpled.Dtos.Comments;

namespace Simpled.Repository
{
    public interface ICommentRepository
    {
        Task<IEnumerable<CommentReadDto>> GetByItemIdAsync(Guid itemId);
        Task<CommentReadDto> CreateAsync(Guid userId, CommentCreateDto dto);
        Task<bool> DeleteAsync(Guid commentId, Guid userId);
        Task<bool> MarkAsResolvedAsync(Guid commentId, Guid userId, bool resolved);
        Task<CommentReadDto?> UpdateAsync(Guid userId, Guid commentId, CommentUpdateDto dto);
    }
}