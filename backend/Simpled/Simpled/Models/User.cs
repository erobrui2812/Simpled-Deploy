namespace Simpled.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public DateTime CreatedAt { get; set; }


        public List<UserRole> Roles { get; set; } = new();


        public List<BoardMember> BoardMembers { get; set; } = new();
    }
}
