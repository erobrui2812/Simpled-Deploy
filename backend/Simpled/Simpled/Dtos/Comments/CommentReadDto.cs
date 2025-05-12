namespace Simpled.Dtos.Comments
{
    /// <summary>
    /// DTO para la lectura de un comentario.
    /// </summary>
    public class CommentReadDto
    {
        /// <summary>
        /// Identificador único del comentario.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Texto del comentario.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el comentario está resuelto.
        /// </summary>
        public bool IsResolved { get; set; }

        /// <summary>
        /// Fecha de creación del comentario.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última edición del comentario (si existe).
        /// </summary>
        public DateTime? EditedAt { get; set; }

        /// <summary>
        /// Identificador del usuario autor del comentario.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nombre del usuario autor del comentario.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// URL del avatar del usuario (opcional).
        /// </summary>
        public string? UserAvatarUrl { get; set; } 
    }
}
