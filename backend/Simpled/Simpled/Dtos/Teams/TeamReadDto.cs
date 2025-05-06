using Simpled.Dtos.Teams.TeamMembers;

namespace Simpled.Dtos.Teams
{
    public class TeamReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = default!;

        public string Role { get; set; } = default!;
        public IEnumerable<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
    }
}
