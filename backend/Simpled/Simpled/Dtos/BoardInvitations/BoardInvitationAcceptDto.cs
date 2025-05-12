namespace Simpled.Dtos.BoardInvitations
{
    /// <summary>
    /// DTO para aceptar una invitación a un tablero mediante token.
    /// </summary>
    public class BoardInvitationAcceptDto
    {
        /// <summary>
        /// Token único de la invitación.
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}