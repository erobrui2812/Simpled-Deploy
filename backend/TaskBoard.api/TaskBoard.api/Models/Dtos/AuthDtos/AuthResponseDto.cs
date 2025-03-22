namespace TaskBoard.api.Models.Dtos.AuthDtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserProfileDto User { get; set; }

    }
}



