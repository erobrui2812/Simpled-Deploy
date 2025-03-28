using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Users
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;
    }
}
