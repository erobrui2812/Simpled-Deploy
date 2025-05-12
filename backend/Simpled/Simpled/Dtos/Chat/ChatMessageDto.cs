namespace Simpled.Dtos.Chat
{
    /// <summary>
    /// DTO de lectura para un mensaje de chat.
    /// </summary>
    public class ChatMessageReadDto
    {
        /// <summary>
        /// Identificador del mensaje.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Sala a la que pertenece este mensaje.
        /// </summary>
        public Guid ChatRoomId { get; set; }

        /// <summary>
        /// Usuario que envió el mensaje.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Texto del mensaje.
        /// </summary>
        public string Text { get; set; } = default!;

        /// <summary>
        /// Fecha y hora de envío (UTC).
        /// </summary>
        public DateTime SentAt { get; set; }
    }
}
