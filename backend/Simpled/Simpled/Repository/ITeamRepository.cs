using Simpled.Dtos.Teams;

namespace Simpled.Repository
{
    public interface ITeamRepository
    {
        Task<IEnumerable<TeamReadDto>> GetAllByUserAsync(Guid userId);
        Task<TeamReadDto?> GetByIdAsync(Guid id);
        Task<TeamReadDto> CreateAsync(TeamCreateDto dto, Guid ownerId);
        Task<bool> UpdateAsync(TeamUpdateDto dto, Guid ownerId);
        Task<bool> DeleteAsync(Guid id, Guid ownerId);
    }
}