using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Registra las acciones realizadas en los ítems del tablero.
    /// </summary>
    public class ActivityLog
    {
        /// <summary>
        /// Identificador único del registro de actividad.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del ítem al que pertenece esta actividad.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Ítem asociado a esta actividad.
        /// </summary>
        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;

        /// <summary>
        /// Identificador del usuario que realizó la acción.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Usuario que realizó la acción.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        /// <summary>
        /// Descripción de la acción realizada.
        /// </summary>
        [Required, MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del campo modificado, si aplica.
        /// </summary>
        [MaxLength(50)]
        public string? Field { get; set; }

        /// <summary>
        /// Valor anterior del campo, si corresponde.
        /// </summary>
        [MaxLength(200)]
        public string? OldValue { get; set; }

        /// <summary>
        /// Nuevo valor del campo, si corresponde.
        /// </summary>
        [MaxLength(200)]
        public string? NewValue { get; set; }

        /// <summary>
        /// Detalles adicionales de la actividad.
        /// </summary>
        [Required, MaxLength(500)]
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se registró la actividad (UTC).
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}