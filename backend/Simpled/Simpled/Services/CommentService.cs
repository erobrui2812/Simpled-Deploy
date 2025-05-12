using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Comments;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exception;
using Simpled.Validators;
using FluentValidation;

namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión de comentarios en ítems.
    /// Implementa ICommentRepository.
    /// </summary>
    public class CommentService : ICommentRepository
    {
        private readonly SimpledDbContext _context;

        public CommentService(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene los comentarios asociados a un ítem.
        /// </summary>
        /// <param name="itemId">ID del ítem.</param>
        /// <returns>Lista de comentarios.</returns>
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

        /// <summary>
        /// Crea un nuevo comentario para un ítem.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="dto">Datos del comentario.</param>
        /// <returns>DTO del comentario creado.</returns>
        public async Task<CommentReadDto> CreateAsync(Guid userId, CommentCreateDto dto)
        {
            var validator = new CommentCreateValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ApiException(validationResult.Errors[0].ErrorMessage, 400);

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

        /// <summary>
        /// Elimina un comentario de un ítem.
        /// </summary>
        /// <param name="commentId">ID del comentario.</param>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>True si la eliminación fue exitosa.</returns>
        public async Task<bool> DeleteAsync(Guid commentId, Guid userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment == null) throw new NotFoundException("Comentario no encontrado o no tienes permisos para eliminarlo.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Marca un comentario como resuelto o no resuelto.
        /// </summary>
        /// <param name="commentId">ID del comentario.</param>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="resolved">Estado de resolución.</param>
        /// <returns>True si la operación fue exitosa.</returns>
        public async Task<bool> MarkAsResolvedAsync(Guid commentId, Guid userId, bool resolved)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment == null) throw new NotFoundException("Comentario no encontrado o no tienes permisos para modificarlo.");

            comment.IsResolved = resolved;
            comment.EditedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Actualiza el texto de un comentario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="commentId">ID del comentario.</param>
        /// <param name="dto">Datos actualizados del comentario.</param>
        /// <returns>DTO actualizado o null si no existe.</returns>
        public async Task<CommentReadDto?> UpdateAsync(Guid userId, Guid commentId, CommentUpdateDto dto)
        {
            if (commentId != dto.CommentId)
                throw new ApiException("El ID del comentario no coincide.", 400);

            var validator = new CommentUpdateValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ApiException(validationResult.Errors[0].ErrorMessage, 400);

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment == null) throw new NotFoundException("Comentario no encontrado o no tienes permisos para editarlo.");

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
