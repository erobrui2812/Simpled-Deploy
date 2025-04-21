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

        public string urlImagen { get; set; } = "placeholder";

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
