using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa la relación entre un usuario y un equipo con un rol determinado.
    /// </summary>
    public class TeamMember
    {
        /// <summary>
        /// Identificador del equipo.
        /// </summary>
        [Required]
        public Guid TeamId { get; set; }

        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Rol del usuario dentro del equipo (máx. 50 caracteres).
        /// </summary>
        [Required, MaxLength(50)]
        public string Role { get; set; } = default!;

        /// <summary>
        /// Equipo al que pertenece el miembro.
        /// </summary>
        public Team? Team { get; set; }

        /// <summary>
        /// Usuario miembro del equipo.
        /// </summary>
        public User? User { get; set; }
    }
}
