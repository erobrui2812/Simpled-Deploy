using System.ComponentModel.DataAnnotations;

namespace TaskBoard.api.Models.Dtos.AuthDtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }
    }
}


