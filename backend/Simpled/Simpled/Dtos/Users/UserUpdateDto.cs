using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Users
{
    public class UserUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        public string? Password { get; set; }
    }
}
