
using Simpled.Dtos.Comments;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar comentarios en ítems.
    /// </summary>
    public interface ICommentRepository
    {
        /// <summary>
        /// Obtiene todos los comentarios asociados a un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <returns>Secuencia de <see cref="CommentReadDto"/>.</returns>
        Task<IEnumerable<CommentReadDto>> GetByItemIdAsync(Guid itemId);

        /// <summary>
        /// Crea un nuevo comentario en un ítem.
        /// </summary>
        /// <param name="userId">Identificador del usuario autor.</param>
        /// <param name="dto">DTO con los datos del comentario.</param>
        /// <returns>DTO <see cref="CommentReadDto"/> del comentario creado.</returns>
        Task<CommentReadDto> CreateAsync(Guid userId, CommentCreateDto dto);

        /// <summary>
        /// Elimina un comentario.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario que intenta eliminar.</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> DeleteAsync(Guid commentId, Guid userId);

        /// <summary>
        /// Marca o desmarca un comentario como resuelto.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario que modifica el estado.</param>
        /// <param name="resolved"><c>true</c> para marcar como resuelto; <c>false</c> para revertir.</param>
        /// <returns><c>true</c> si la operación fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> MarkAsResolvedAsync(Guid commentId, Guid userId, bool resolved);

        /// <summary>
        /// Actualiza el texto de un comentario existente.
        /// </summary>
        /// <param name="userId">Identificador del usuario autor.</param>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="dto">DTO con el nuevo texto.</param>
        /// <returns>DTO <see cref="CommentReadDto"/> actualizado, o <c>null</c> si no existe.</returns>
        Task<CommentReadDto?> UpdateAsync(Guid userId, Guid commentId, CommentUpdateDto dto);
    }
}
