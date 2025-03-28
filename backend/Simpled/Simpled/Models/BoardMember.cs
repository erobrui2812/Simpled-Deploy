using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class BoardMember
    {
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }

        [Required]
        [RegularExpression("admin|editor|viewer")]
        public string Role { get; set; } = default!;

        public Board? Board { get; set; }
        public User? User { get; set; }
    }
}
