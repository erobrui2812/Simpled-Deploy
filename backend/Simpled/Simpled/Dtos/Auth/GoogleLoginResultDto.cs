namespace Simpled.Dtos.Auth
{
    public class GoogleLoginResultDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public bool NeedsVerification { get; set; }
        public string ErrorMessage { get; set; }
    }
}
