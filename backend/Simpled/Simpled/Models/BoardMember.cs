using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa la membresía de un usuario en un tablero.
    /// </summary>
    public class BoardMember
    {
        /// <summary>
        /// Identificador del tablero.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Rol del usuario dentro del tablero.
        /// </summary>
        [Required]
        [RegularExpression("admin|editor|viewer")]
        public string Role { get; set; } = default!;

        /// <summary>
        /// Tablero asociado a esta membresía.
        /// </summary>
        public Board? Board { get; set; }

        /// <summary>
        /// Usuario asociado a esta membresía.
        /// </summary>
        public User? User { get; set; }
    }
}
