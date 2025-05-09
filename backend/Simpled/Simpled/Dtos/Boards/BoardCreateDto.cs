using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Boards
{
    /// <summary>
    /// DTO para la creación de un tablero.
    /// </summary>
    public class BoardCreateDto
    {
        /// <summary>
        /// Nombre del tablero.
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
