using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Subtasks
{
    /// <summary>
    /// DTO para la creación de una subtarea.
    /// </summary>
    public class SubtaskCreateDto
    {
        /// <summary>
        /// Identificador del ítem al que pertenece la subtarea.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Título de la subtarea.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;
    }
}
