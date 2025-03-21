using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Models
{
    public class Invitation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string Code { get; set; } // QR/Enlace
        public DateTime Expiration { get; set; }
        public string Role { get; set; } // "Viewer", "Editor"

        public Board Board { get; set; }
    }
}


