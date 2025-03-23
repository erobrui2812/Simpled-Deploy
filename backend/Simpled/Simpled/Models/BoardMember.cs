namespace Simpled.Models
{
    public class BoardMember
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = default!;  // "admin|editor|viewer"

        public Board? Board { get; set; }
        public User? User { get; set; }
    }
}
