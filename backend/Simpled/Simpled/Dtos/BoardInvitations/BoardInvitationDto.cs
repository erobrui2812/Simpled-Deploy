namespace Simpled.Dtos.BoardInvitations
{

    /// <summary>
    /// DTO para crear una nueva invitación a un tablero.
    /// </summary>
    public class BoardInvitationCreateDto
    {
        /// <summary>
        /// Identificador del tablero al que se invita.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Correo electrónico del invitado.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Rol asignado al invitado (admin, editor o viewer).
        /// </summary>
        public string Role { get; set; } = "viewer";
    }
}
