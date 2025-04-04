namespace Simpled.Dtos.BoardInvitations
{
    public class BoardInvitationReadDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string BoardName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool Accepted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
