using Simpled.Dtos.TeamInvitations;
using Simpled.Models;

namespace Simpled.Repository
{
    public interface ITeamInvitationRepository
    {
        Task<IEnumerable<TeamInvitationReadDto>> GetAllByEmailAsync(string email);
        Task<TeamInvitationReadDto?> GetByTokenAsync(string token);
        Task<TeamInvitation> CreateAsync(TeamInvitationCreateDto dto);
        Task<bool> AcceptAsync(string token, Guid userId);
        Task<bool> RejectAsync(string token);
    }
}
