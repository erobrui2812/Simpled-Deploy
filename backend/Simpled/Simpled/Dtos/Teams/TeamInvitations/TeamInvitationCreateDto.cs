using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.TeamInvitations
{
    /// <summary>
    /// DTO para crear una nueva invitación a un equipo.
    /// </summary>
    public class TeamInvitationCreateDto
    {
        /// <summary>
        /// Identificador del equipo al que se envía la invitación.
        /// </summary>
        [Required]
        public Guid TeamId { get; set; }

        /// <summary>
        /// Correo electrónico del usuario invitado.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}