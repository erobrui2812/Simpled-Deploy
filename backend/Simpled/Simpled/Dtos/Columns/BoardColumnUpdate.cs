using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Columns
{
    /// <summary>
    /// DTO para la actualización de una columna de tablero.
    /// </summary>
    public class BoardColumnUpdateDto
    {
        /// <summary>
        /// Identificador único de la columna.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del tablero al que pertenece la columna.
        /// </summary>
        [Required]
        public Guid BoardId { get; set; }

        /// <summary>
        /// Nuevo título de la columna.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Nuevo orden de la columna en el tablero.
        /// </summary>
        public int Order { get; set; }
    }
}
