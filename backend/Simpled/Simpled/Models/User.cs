using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Simpled.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = "Dummy";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string PasswordHash { get; set; } = default!;

        public string ImageUrl { get; set; } = "/images/default/avatar-default.jpg";

        public DateTime CreatedAt { get; set; }

        public List<UserRole> Roles { get; set; } = new();
        public List<BoardMember> BoardMembers { get; set; } = new();

        public int CreatedBoardsCount { get; set; }
        public int CreatedTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int TeamsCount { get; set; }

        public ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    }
}
