namespace Simpled.Dtos.BoardMembers
{
 /// <summary>
    /// DTO para actualizar el rol de un miembro en un tablero.
    /// </summary>
    public class BoardMemberUpdateDto
    {
        /// <summary>
        /// Identificador del tablero.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Identificador del usuario miembro del tablero.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nuevo rol asignado al usuario (admin, editor o viewer).
        /// </summary>
        public string Role { get; set; } = default!;
    }
}

