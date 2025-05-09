using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Columns
{
    /// <summary>
    /// DTO para la creación de una columna de tablero.
    /// </summary>
    public class BoardColumnCreateDto
    {
        /// <summary>
        /// Identificador del tablero al que pertenece la columna.
        /// </summary>
        [Required]
        public Guid BoardId { get; set; }

        /// <summary>
        /// Título de la columna.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Orden de la columna en el tablero.
        /// </summary>
        public int Order { get; set; }
    }
}
