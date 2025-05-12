using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simpled.Models
{
    /// <summary>
    /// Representa una sala de chat asociada a un tablero o a un equipo.
    /// </summary>
    public class ChatRoom
    {
        /// <summary>
        /// Identificador único de la sala de chat.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo de sala ("Team" o "Board").
        /// </summary>
        [Required, MaxLength(20)]
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de la entidad a la que pertenece (TeamId o BoardId).
        /// </summary>
        [Required]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Fecha y hora de creación de la sala (UTC).
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Mensajes pertenecientes a esta sala.
        /// </summary>
        public List<ChatMessage> ChatMessages { get; set; } = new();


    }
}
