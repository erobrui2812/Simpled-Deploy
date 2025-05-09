using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Items
{
     /// <summary>
    /// DTO para creación de un ítem (tarea).
    /// </summary>
    public class ItemCreateDto
    {
        /// <summary>
        /// Título de la tarea.
        /// </summary>
        [Required, MaxLength(100)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Descripción opcional de la tarea.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Fecha límite para completar la tarea (UTC), si aplica.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Fecha de inicio de la tarea (UTC).
        /// </summary>
        [Required]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Identificador de la columna donde se crea la tarea.
        /// </summary>
        public Guid ColumnId { get; set; }

        /// <summary>
        /// Estado inicial de la tarea: pending, in-progress, completed o delayed.
        /// </summary>
        [Required, RegularExpression("pending|in-progress|completed|delayed")]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// Identificador del usuario asignado a la tarea, si existe.
        /// </summary>
        public Guid? AssigneeId { get; set; }
    }
}
