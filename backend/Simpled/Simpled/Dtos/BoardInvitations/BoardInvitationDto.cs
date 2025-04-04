namespace Simpled.Dtos.BoardInvitations
{
    public class BoardInvitationCreateDto
    {
        public Guid BoardId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "viewer";
    }
}
