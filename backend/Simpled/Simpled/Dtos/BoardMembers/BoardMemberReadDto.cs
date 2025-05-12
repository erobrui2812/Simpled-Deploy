namespace Simpled.Dtos.BoardMembers
{
    /// <summary>
    /// DTO para lectura de miembros de un tablero.
    /// </summary>
    public class BoardMemberReadDto
    {
        /// <summary>
        /// Identificador del tablero.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Identificador del usuario miembro.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Rol del usuario en el tablero.
        /// </summary>
        public string Role { get; set; } = default!;
    }
}
