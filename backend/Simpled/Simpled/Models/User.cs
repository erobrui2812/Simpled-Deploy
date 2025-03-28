using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string PasswordHash { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public List<UserRole> Roles { get; set; } = new();
        public List<BoardMember> BoardMembers { get; set; } = new();
    }
}
