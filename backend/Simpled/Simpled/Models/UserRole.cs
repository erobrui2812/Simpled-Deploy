using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class UserRole
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [Required]
        [RegularExpression("admin|editor|viewer")]
        public string Role { get; set; } = default!;

        public User? User { get; set; }
    }
}
