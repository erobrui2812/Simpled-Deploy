namespace Simpled.Dtos.TeamInvitations
{
    /// <summary>
    /// DTO para lectura de invitaciones a un equipo.
    /// </summary>
    public class TeamInvitationReadDto
    {
        /// <summary>
        /// Identificador único de la invitación.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del equipo invitado.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Nombre del equipo invitado.
        /// </summary>
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// Token único generado para aceptar la invitación.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la invitación ya fue aceptada.
        /// </summary>
        public bool Accepted { get; set; }

        /// <summary>
        /// Fecha de creación de la invitación (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}