using System.Diagnostics.CodeAnalysis;
using Simpled.Dtos.Teams;

namespace Simpled.Dtos.Users
{
    public class UserReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } = default!;
        public string imageUrl { get; set; }
        public int achievementsCompleted { get; set; }
        public List<TeamDto> teams { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
