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

        public int TablerosCreados { get; set; }
        public int TareasCreadas { get; set; }
        public int TareasCompletadas { get; set; }
        public int EquiposUnidos { get; set; }

        public ICollection<UserAchievement> Logros { get; set; } = new List<UserAchievement>();

    }
}
