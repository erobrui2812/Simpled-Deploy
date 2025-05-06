using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.TeamInvitations
{
    public class TeamInvitationCreateDto
    {
        [Required]
        public Guid TeamId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}