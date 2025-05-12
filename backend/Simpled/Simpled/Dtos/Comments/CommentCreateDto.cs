using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Comments
{
    /// <summary>
    /// DTO para la creación de un comentario.
    /// </summary>
    public class CommentCreateDto
    {
        /// <summary>
        /// Identificador del ítem al que pertenece el comentario.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Texto del comentario.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;
    }
}
