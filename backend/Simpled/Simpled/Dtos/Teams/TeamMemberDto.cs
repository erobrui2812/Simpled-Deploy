namespace Simpled.Dtos.Teams
{
    public class TeamMemberDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
