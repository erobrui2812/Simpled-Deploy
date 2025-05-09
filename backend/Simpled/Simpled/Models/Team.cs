using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa un equipo de trabajo.
    /// </summary>
    public class Team
    {
        /// <summary>
        /// Identificador único del equipo.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del equipo.
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Identificador del usuario propietario del equipo.
        /// </summary>
        [Required]
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Usuario propietario del equipo.
        /// </summary>
        public User? Owner { get; set; }

        /// <summary>
        /// Miembros que forman parte del equipo.
        /// </summary>
        public List<TeamMember> Members { get; set; } = new();
    }
}
