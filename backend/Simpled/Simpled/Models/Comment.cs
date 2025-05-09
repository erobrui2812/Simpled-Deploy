using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simpled.Models
{
   /// <summary>
    /// Representa un comentario realizado por un usuario en un ítem.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Identificador único del comentario.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del ítem al que se asocia el comentario.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Ítem asociado al comentario.
        /// </summary>
        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;

        /// <summary>
        /// Identificador del usuario que hace el comentario.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Usuario que hizo el comentario.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        /// <summary>
        /// Texto del comentario (máx. 1000 caracteres).
        /// </summary>
        [Required, MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el comentario ha sido resuelto.
        /// </summary>
        public bool IsResolved { get; set; } = false;

        /// <summary>
        /// Fecha y hora de creación del comentario (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última edición, si aplica.
        /// </summary>
        public DateTime? EditedAt { get; set; }
    }
}
