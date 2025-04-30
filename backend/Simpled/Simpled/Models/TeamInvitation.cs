using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class TeamInvitation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = Guid.NewGuid().ToString();

        public bool Accepted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Team? Team { get; set; }
    }
}
