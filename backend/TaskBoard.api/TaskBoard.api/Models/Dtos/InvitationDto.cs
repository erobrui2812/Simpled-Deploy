using System.ComponentModel.DataAnnotations;

namespace TaskBoard.api.Models.Dtos
{
    public class InvitationCreateDto
    {
        [Required] public Guid BoardId { get; set; }
        [Required] public string Role { get; set; }
        public int ExpireInHours { get; set; } = 4;
        public int MaxUses { get; set; } = 5;
    }

    public class InvitationResponseDto
    {
        public string Code { get; set; }
        public string JoinUrl { get; set; } //para el front
        public DateTime Expiration { get; set; }
        public int RemainingUses { get; set; }
    }

    public class JoinByCodeDto
    {
        [Required] public string Code { get; set; }
    }
}


