using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
     /// <summary>
    /// Define los roles posibles de un usuario global.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Identificador único de la relación rol-usuario.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Rol asignado (admin, editor o viewer).
        /// </summary>
        [Required, RegularExpression("admin|editor|viewer")]
        public string Role { get; set; } = default!;

        /// <summary>
        /// Usuario al que pertenece el rol.
        /// </summary>
        public User? User { get; set; }
    }
}
