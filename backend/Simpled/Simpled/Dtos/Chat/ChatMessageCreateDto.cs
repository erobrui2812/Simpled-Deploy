using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Chat
{
    /// <summary>
    /// DTO para la creación de un nuevo mensaje de chat.
    /// </summary>
    public class ChatMessageCreateDto
    {
        /// <summary>
        /// Sala a la que se envía el mensaje.
        /// </summary>
        [Required]
        public Guid ChatRoomId { get; set; }

        /// <summary>
        /// Texto del mensaje.
        /// </summary>
        [Required, MaxLength(1000)]
        public string Text { get; set; } = default!;
    }
}
