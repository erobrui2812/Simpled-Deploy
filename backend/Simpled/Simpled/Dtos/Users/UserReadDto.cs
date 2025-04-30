using Simpled.Dtos.Teams;

namespace Simpled.Dtos.Users
{
    public class UserReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public int AchievementsCompleted { get; set; }
        public List<TeamReadDto> Teams { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
