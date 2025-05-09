using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Boards
{
    /// <summary>
    /// DTO para la actualización de un tablero existente.
    /// </summary>
    public class BoardUpdateDto
    {
        /// <summary>
        /// Identificador único del tablero.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Nuevo nombre del tablero.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Indica si el tablero es público.
        /// </summary>
        public bool IsPublic { get; set; }
    }
}
