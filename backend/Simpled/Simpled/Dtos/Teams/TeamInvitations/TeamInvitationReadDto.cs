namespace Simpled.Dtos.TeamInvitations
{
    public class TeamInvitationReadDto
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool Accepted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}