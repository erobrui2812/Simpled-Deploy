using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class BoardInvitation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BoardId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Role { get; set; } = "viewer";

        public bool Accepted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relaciones
        public Board? Board { get; set; }
    }
}
