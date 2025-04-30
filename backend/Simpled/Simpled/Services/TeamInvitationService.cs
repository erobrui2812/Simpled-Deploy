
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.TeamInvitations;
using Simpled.Exception;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    public class TeamInvitationService : ITeamInvitationRepository
    {
        private readonly SimpledDbContext _context;

        public TeamInvitationService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeamInvitationReadDto>> GetAllByEmailAsync(string email)
        {
            return await _context.TeamInvitations
                .Where(i => i.Email == email && !i.Accepted)
                .Include(i => i.Team)
                .Select(i => new TeamInvitationReadDto
                {
                    Id = i.Id,
                    TeamId = i.TeamId,
                    TeamName = i.Team!.Name,
                    Token = i.Token,
                    Accepted = i.Accepted,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TeamInvitationReadDto?> GetByTokenAsync(string token)
        {
            var i = await _context.TeamInvitations
                .Include(i => i.Team)
                .FirstOrDefaultAsync(i => i.Token == token);
            if (i == null) return null;

            return new TeamInvitationReadDto
            {
                Id = i.Id,
                TeamId = i.TeamId,
                TeamName = i.Team!.Name,
                Token = i.Token,
                Accepted = i.Accepted,
                CreatedAt = i.CreatedAt
            };
        }

        public async Task<TeamInvitation> CreateAsync(TeamInvitationCreateDto dto)
        {
            if (await _context.TeamInvitations.AnyAsync(i =>
                    i.TeamId == dto.TeamId && i.Email == dto.Email && !i.Accepted))
                throw new ApiException("Ya existe invitación pendiente para este usuario.", 409);

            var inv = new TeamInvitation
            {
                Id = Guid.NewGuid(),
                TeamId = dto.TeamId,
                Email = dto.Email.ToLower(),
                Token = Guid.NewGuid().ToString()
            };
            _context.TeamInvitations.Add(inv);
            await _context.SaveChangesAsync();
            return inv;
        }

        public async Task<bool> AcceptAsync(string token, Guid userId)
        {
            var inv = await _context.TeamInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.Accepted);
            if (inv == null)
                throw new NotFoundException("Invitación no encontrada o ya procesada.");

            // add to TeamMembers
            _context.TeamMembers.Add(new TeamMember
            {
                TeamId = inv.TeamId,
                UserId = userId,
                Role = "viewer"
            });

            inv.Accepted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAsync(string token)
        {
            var inv = await _context.TeamInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.Accepted);
            if (inv == null)
                throw new NotFoundException("Invitación no encontrada o ya procesada.");

            _context.TeamInvitations.Remove(inv);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
