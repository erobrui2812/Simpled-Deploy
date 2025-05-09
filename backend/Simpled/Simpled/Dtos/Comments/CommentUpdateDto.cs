using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Comments
{
    /// <summary>
    /// DTO para la actualización de un comentario existente.
    /// </summary>
    public class CommentUpdateDto
    {
        /// <summary>
        /// Identificador del ítem al que pertenece el comentario.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Identificador del comentario a actualizar.
        /// </summary>
        [Required]
        public Guid CommentId { get; set; }

        /// <summary>
        /// Nuevo texto del comentario.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;
    }
}
