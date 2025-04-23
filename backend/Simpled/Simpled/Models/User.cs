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

        public string imageUrl { get; set; } = "placeholder";

        public DateTime CreatedAt { get; set; }

        public List<UserRole> Roles { get; set; } = new();
        public List<BoardMember> BoardMembers { get; set; } = new();

        public int createdBoardsCount { get; set; }
        public int createdTasksCount { get; set; }
        public int completedTasksCount { get; set; }
        public int teamsCount { get; set; }

        public ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    }
}
