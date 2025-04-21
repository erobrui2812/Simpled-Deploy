namespace Simpled.Dtos.Auth
{
    public class LoginResultDto
    {
        public string Token { get; set; } = default!;
        public string UserId { get; set; } = default!;
    }
}