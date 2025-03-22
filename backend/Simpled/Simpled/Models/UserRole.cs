namespace Simpled.Models
{
    public class UserRole
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = default!;


        public User? User { get; set; }
    }
}
