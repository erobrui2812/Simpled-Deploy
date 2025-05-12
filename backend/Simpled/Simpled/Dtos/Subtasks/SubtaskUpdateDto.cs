using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Subtasks
{
    /// <summary>
    /// DTO para la actualización de una subtarea existente.
    /// </summary>
    public class SubtaskUpdateDto
    {
        /// <summary>
        /// Identificador único de la subtarea.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del ítem al que pertenece la subtarea.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Nuevo título de la subtarea.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Indica si la subtarea está completada.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
