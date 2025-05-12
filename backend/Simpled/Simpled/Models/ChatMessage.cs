using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simpled.Models
{
    /// <summary>
    /// Representa un mensaje enviado en una sala de chat.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Identificador único del mensaje.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Sala de chat a la que pertenece este mensaje.
        /// </summary>
        [Required]
        public Guid ChatRoomId { get; set; }

        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; } = null!;

        /// <summary>
        /// Usuario que envió el mensaje.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Contenido del mensaje.
        /// </summary>
        [Required, MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se envió el mensaje (UTC).
        /// </summary>
        [Required]
        public DateTime SentAt { get; set; }
    }
}
