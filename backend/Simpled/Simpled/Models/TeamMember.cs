using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class TeamMember
    {
        [Required]
        public Guid TeamId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, MaxLength(50)]
        public string Role { get; set; } = default!; 

        public Team? Team { get; set; }
        public User? User { get; set; }
    }
}
