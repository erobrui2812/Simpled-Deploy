using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa una entrada de contenido asociada a un ítem.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Identificador único del contenido.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del ítem relacionado.
        /// </summary>
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Tipo de contenido (por ejemplo: texto, imagen, casilla).
        /// </summary>
        [Required]
        [RegularExpression("text|image|checkbox")]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Valor del contenido.
        /// </summary>
        [Required]
        [MaxLength(2048)]
        public string Value { get; set; } = default!;

        /// <summary>
        /// Ítem al que pertenece este contenido.
        /// </summary>
        public Item? Item { get; set; }
    }

}
