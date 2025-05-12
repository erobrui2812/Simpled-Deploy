namespace Simpled.Dtos.Chat
{
    /// <summary>
    /// DTO de lectura para una sala
    /// </summary>
    public class ChatRoomReadDto
    {
        /// <summary>
        /// Identificador de la sala.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo de sala: "Team" o "Board".
        /// </summary>
        public string RoomType { get; set; } = default!;

        /// <summary>
        /// Identificador del Team o Board asociado.
        /// </summary>
        public Guid EntityId { get; set; }
    }
}
