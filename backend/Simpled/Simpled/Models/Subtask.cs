using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa una subtarea relacionada con un ítem.
    /// </summary>
    public class Subtask
    {
        /// <summary>
        /// Identificador único de la subtarea.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del ítem al que pertenece la subtarea.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Título de la subtarea (máx. 200 caracteres).
        /// </summary>
        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Indica si la subtarea está completada.
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Ítem al que pertenece la subtarea.
        /// </summary>
        public Item Item { get; set; } = default!;
    }
}
