using System.ComponentModel.DataAnnotations;

namespace TaskBoard.api.Models
{
    public class JwtSettings
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Issuer { get; set; }

        [Required]
        public string Audience { get; set; }
    }
}

