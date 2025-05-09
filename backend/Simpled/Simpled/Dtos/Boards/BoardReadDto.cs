namespace Simpled.Dtos.Boards
{
    /// <summary>
    /// DTO para la lectura de un tablero.
    /// </summary>
    public class BoardReadDto
    {
        /// <summary>
        /// Identificador único del tablero.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del tablero.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del usuario propietario del tablero.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Indica si el tablero es público.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Rol del usuario en el tablero (opcional).
        /// </summary>
        public string? UserRole { get; set; } 

        /// <summary>
        /// Indica si el tablero es favorito para el usuario (opcional).
        /// </summary>
        public bool? IsFavorite { get; set; }
    }

}
