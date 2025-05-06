
using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }

        public List<TeamMember> Members { get; set; } = new();
    }
}
