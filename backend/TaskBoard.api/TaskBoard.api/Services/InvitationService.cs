using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using TaskBoard.api.Data;
using TaskBoard.api.Models;
using TaskBoard.api.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace TaskBoard.api.Services
{
    public class InvitationService
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvitationService(
            AppDbContext context,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor) // Añade esta línea
        {
            _context = context;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<InvitationResponseDto> CreateInvitation(
            InvitationCreateDto dto,
            Guid creatorId)
        {
            var board = await _context.Boards.FindAsync(dto.BoardId);
            if (board?.OwnerId != creatorId)
                throw new UnauthorizedAccessException("Solo el propietario puede crear invitaciones");

            var invitation = new Invitation
            {
                BoardId = dto.BoardId,
                Code = GenerateUniqueCode(),
                Role = dto.Role,
                Expiration = DateTime.UtcNow.AddDays(dto.ExpireInHours),
                MaxUses = dto.MaxUses,
                CreatorId = creatorId
            };

            await _context.Invitations.AddAsync(invitation);
            await _context.SaveChangesAsync();

            return new InvitationResponseDto
            {
                Code = invitation.Code,
                JoinUrl = _linkGenerator.GetUriByAction(
                    _httpContextAccessor.HttpContext,
                    action: "JoinByCode",
                    controller: "Invitation",
                    values: new { code = invitation.Code }),
                Expiration = invitation.Expiration,
                RemainingUses = invitation.MaxUses - invitation.UsedCount
            };
        }

        public async Task<Guid> JoinByCode(string code, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invitation = await _context.Invitations
                    .FirstOrDefaultAsync(i =>
                        i.Code == code &&
                        i.IsActive &&
                        i.Expiration > DateTime.UtcNow &&
                        i.UsedCount < i.MaxUses);

                if (invitation == null)
                    throw new InvalidOperationException("Código inválido o expirado");

                var existingMember = await _context.BoardMembers
                    .AnyAsync(m => m.BoardId == invitation.BoardId && m.UserId == userId);

                if (!existingMember)
                {
                    _context.BoardMembers.Add(new BoardMember
                    {
                        BoardId = invitation.BoardId,
                        UserId = userId,
                        Role = invitation.Role
                    });

                    invitation.UsedCount++;
                    invitation.IsActive = invitation.UsedCount < invitation.MaxUses;

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return invitation.BoardId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private string GenerateUniqueCode() => Guid.NewGuid().ToString("N")[..8].ToUpper();
    }
}


