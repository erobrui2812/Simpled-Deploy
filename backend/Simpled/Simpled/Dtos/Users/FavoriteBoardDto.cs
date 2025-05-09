namespace Simpled.Dtos.Users
{
    /// <summary>
    /// DTO para marcar un tablero como favorito.
    /// </summary>
    public class FavoriteBoardDto
    {
        /// <summary>
        /// Identificador del tablero a marcar como favorito.
        /// </summary>
        public Guid BoardId { get; set; }
    }
}
