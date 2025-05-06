using Simpled.Dtos.Teams.TeamMembers;

namespace Simpled.Repository
{
    public interface ITeamMemberRepository
    {
        Task<IEnumerable<TeamMemberDto>> GetMembersAsync(Guid teamId);
        Task AddMemberAsync(TeamMemberCreateDto dto, Guid ownerId);
        Task<bool> UpdateMemberAsync(TeamMemberUpdateDto dto, Guid ownerId);
        Task<bool> RemoveMemberAsync(Guid teamId, Guid userId, Guid ownerId);
    }
}
