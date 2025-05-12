namespace Simpled.Models
{
 /// <summary>
    /// Representa un tablero marcado como favorito por un usuario.
    /// </summary>
    public class FavoriteBoards
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Identificador del tablero.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Usuario que marcó el tablero como favorito.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Tablero marcado como favorito.
        /// </summary>
        public Board? Board { get; set; }
    }
}
