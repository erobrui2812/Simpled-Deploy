using Simpled.Dtos.BoardInvitations;

namespace Simpled.Repository
{
    public interface IBoardInvitationRepository
    {
        Task<IEnumerable<BoardInvitationReadDto>> GetAllByEmailAsync(string email);
        Task<BoardInvitationReadDto?> GetByTokenAsync(string token);
        Task CreateAsync(BoardInvitationCreateDto dto);
        Task<bool> AcceptAsync(string token, Guid userId);
        Task<bool> RejectAsync(string token);
    }
}
