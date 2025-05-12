namespace Simpled.Dtos.TeamInvitations
{
    /// <summary>
    /// DTO para aceptar una invitación a un equipo mediante token.
    /// </summary>
    public class TeamInvitationAcceptDto
    {
        /// <summary>
        /// Token único que identifica la invitación.
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}