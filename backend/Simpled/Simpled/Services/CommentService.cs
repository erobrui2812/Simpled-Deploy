using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Comments;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    public class CommentService : ICommentRepository
    {
        private readonly SimpledDbContext _context;

        public CommentService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentReadDto>> GetByItemIdAsync(Guid itemId)
        {
            return await _context.Comments
                .Where(c => c.ItemId == itemId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentReadDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    EditedAt = c.EditedAt,
                    IsResolved = c.IsResolved,
                    UserId = c.UserId,
                    UserName = c.User.Email,
                    UserAvatarUrl = null
                })
                .ToListAsync();
        }

        public async Task<CommentReadDto> CreateAsync(Guid userId, CommentCreateDto dto)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                ItemId = dto.ItemId,
                Text = dto.Text,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new CommentReadDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                IsResolved = comment.IsResolved,
                UserId = comment.UserId,
                UserName = (await _context.Users.FindAsync(userId))?.Email ?? "Usuario",
                UserAvatarUrl = null
            };
        }

        public async Task<bool> DeleteAsync(Guid commentId, Guid userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsResolvedAsync(Guid commentId, Guid userId, bool resolved)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment == null) return false;

            comment.IsResolved = resolved;
            comment.EditedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CommentReadDto?> UpdateAsync(Guid userId, Guid commentId, CommentUpdateDto dto)
        {
            if (commentId != dto.CommentId)
                return null;

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment == null) return null;

            comment.Text = dto.Text;
            comment.EditedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new CommentReadDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                EditedAt = comment.EditedAt,
                IsResolved = comment.IsResolved,
                UserId = comment.UserId,
                UserName = (await _context.Users.FindAsync(userId))?.Email ?? "Usuario",
                UserAvatarUrl = null
            };
        }
    }

}
