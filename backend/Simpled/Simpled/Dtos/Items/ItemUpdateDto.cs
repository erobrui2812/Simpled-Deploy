using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Items
{
    /// <summary>
    /// DTO para la actualización de un ítem existente.
    /// </summary>
    public class ItemUpdateDto
    {
        /// <summary>
        /// Identificador único del ítem.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Nuevo título del ítem.
        /// </summary>
        [Required, MaxLength(100)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Nueva descripción del ítem (opcional).
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Nueva fecha de vencimiento del ítem (opcional).
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Nueva fecha de inicio del ítem (opcional).
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Identificador de la columna a la que pertenece el ítem.
        /// </summary>
        [Required]
        public Guid ColumnId { get; set; }

        /// <summary>
        /// Nuevo estado del ítem.
        /// </summary>
        [Required]
        [RegularExpression("pending|in-progress|completed|delayed")]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// Identificador del usuario asignado (opcional).
        /// </summary>
        public Guid? AssigneeId { get; set; }
    }
}
