namespace Simpled.Dtos.BoardInvitations
{
      /// <summary>
    /// DTO para lectura de invitaciones enviadas a un tablero.
    /// </summary>
    public class BoardInvitationReadDto
    {
        /// <summary>
        /// Identificador único de la invitación.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del tablero invitado.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Nombre del tablero invitado.
        /// </summary>
        public string BoardName { get; set; } = string.Empty;

        /// <summary>
        /// Rol asignado al invitado.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Token único utilizado para aceptar la invitación.
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
