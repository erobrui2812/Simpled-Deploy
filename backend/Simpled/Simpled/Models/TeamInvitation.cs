using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa una invitación a un usuario para unirse a un equipo.
    /// </summary>
    public class TeamInvitation
    {
        /// <summary>
        /// Identificador único de la invitación.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del equipo al que invita la notificación.
        /// </summary>
        [Required]
        public Guid TeamId { get; set; }

        /// <summary>
        /// Correo electrónico del invitado.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Token único para aceptar la invitación.
        /// </summary>
        [Required]
        public string Token { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Indica si la invitación fue aceptada.
        /// </summary>
        public bool Accepted { get; set; } = false;

        /// <summary>
        /// Fecha de creación de la invitación (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Equipo asociado a la invitación.
        /// </summary>
        public Team? Team { get; set; }
    }
}
