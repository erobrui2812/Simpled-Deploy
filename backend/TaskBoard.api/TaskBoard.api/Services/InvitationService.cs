using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using TaskBoard.api.Data;
using TaskBoard.api.Models;
using TaskBoard.api.Models.Dtos;

namespace TaskBoard.api.Services
{
    public class InvitationService
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public InvitationService(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        public async Task<InvitationResponseDto> CreateInvitation(InvitationCreateDto dto, Guid creatorId)
        {
            var board = await _context.Boards
                .FirstOrDefaultAsync(b => b.Id == dto.BoardId && b.OwnerId == creatorId);

            if (board == null) throw new UnauthorizedAccessException();

            var invitation = new Invitation
            {
                BoardId = dto.BoardId,
                Code = GenerateUniqueCode(),
                Expiration = DateTime.UtcNow.AddHours(dto.ExpireInHours),
                Role = dto.Role
            };

            await _context.Invitations.AddAsync(invitation);
            await _context.SaveChangesAsync();

            return new InvitationResponseDto
            {
                Code = invitation.Code,
                JoinUrl = _linkGenerator.GetPathByAction("JoinByCode", "Board", new { code = invitation.Code }),
                Expiration = invitation.Expiration
            };
        }

        private string GenerateUniqueCode() => Guid.NewGuid().ToString("N")[..8].ToUpper();
    }
}



