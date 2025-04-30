using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Teams
{
    public class TeamMemberUpdateDto
    {
        [Required]
        public Guid TeamId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, MaxLength(50)]
        public string Role { get; set; } = default!;
    }
}