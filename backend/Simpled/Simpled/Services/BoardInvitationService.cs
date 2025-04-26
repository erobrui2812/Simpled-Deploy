using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.BoardInvitations;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exception;

namespace Simpled.Services
{
    public class BoardInvitationService : IBoardInvitationRepository
    {
        private readonly SimpledDbContext _context;

        public BoardInvitationService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoardInvitationReadDto>> GetAllByEmailAsync(string email)
        {
            return await _context.BoardInvitations
                .Where(i => i.Email == email && !i.Accepted)
                .Include(i => i.Board)
                .Select(i => new BoardInvitationReadDto
                {
                    Id = i.Id,
                    BoardId = i.BoardId,
                    BoardName = i.Board!.Name,
                    Role = i.Role,
                    Token = i.Token,
                    Accepted = i.Accepted,
                    CreatedAt = i.CreatedAt
                }).ToListAsync();
        }

        public async Task<BoardInvitationReadDto?> GetByTokenAsync(string token)
        {
            var i = await _context.BoardInvitations.Include(i => i.Board)
                .FirstOrDefaultAsync(i => i.Token == token);
            if (i == null) return null;

            return new BoardInvitationReadDto
            {
                Id = i.Id,
                BoardId = i.BoardId,
                BoardName = i.Board!.Name,
                Role = i.Role,
                Token = i.Token,
                Accepted = i.Accepted,
                CreatedAt = i.CreatedAt
            };
        }

        public async Task<BoardInvitation> CreateAsync(BoardInvitationCreateDto dto)
        {
            var exists = await _context.BoardInvitations.AnyAsync(i =>
                i.BoardId == dto.BoardId && i.Email == dto.Email && !i.Accepted);

            if (exists)
                throw new ApiException("Ya existe una invitación pendiente para este usuario en este tablero.", 409);

            var invitation = new BoardInvitation
            {
                Id = Guid.NewGuid(),
                BoardId = dto.BoardId,
                Email = dto.Email.ToLower(),
                Role = dto.Role,
                Token = Guid.NewGuid().ToString()
            };

            _context.BoardInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            return invitation;
        }

        public async Task<bool> AcceptAsync(string token, Guid userId)
        {
            var invitation = await _context.BoardInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.Accepted);

            if (invitation == null)
                throw new NotFoundException("Invitación no encontrada o ya aceptada.");

            _context.BoardMembers.Add(new BoardMember
            {
                BoardId = invitation.BoardId,
                UserId = userId,
                Role = invitation.Role
            });

            invitation.Accepted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectAsync(string token)
        {
            var invitation = await _context.BoardInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.Accepted);

            if (invitation == null)
                throw new NotFoundException("Invitación no encontrada o ya procesada.");

            _context.BoardInvitations.Remove(invitation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
