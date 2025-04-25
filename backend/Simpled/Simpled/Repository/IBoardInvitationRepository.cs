using Simpled.Dtos.BoardInvitations;
using Simpled.Models;

namespace Simpled.Repository
{
    public interface IBoardInvitationRepository
    {
        Task<IEnumerable<BoardInvitationReadDto>> GetAllByEmailAsync(string email);
        Task<BoardInvitationReadDto?> GetByTokenAsync(string token);
        Task<BoardInvitation> CreateAsync(BoardInvitationCreateDto dto); 
        Task<bool> AcceptAsync(string token, Guid userId);
        Task<bool> RejectAsync(string token);
    }
}
