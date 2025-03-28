namespace Simpled.Dtos.Users
{
    public class UserReadDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
