using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.BoardMembers;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exceptions;

namespace Simpled.Services
{
    public class BoardMemberService : IBoardMemberRepository
    {
        private readonly SimpledDbContext _context;

        public BoardMemberService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoardMemberReadDto>> GetAllAsync()
        {
            return await _context.BoardMembers
                .Select(m => new BoardMemberReadDto
                {
                    BoardId = m.BoardId,
                    UserId = m.UserId,
                    Role = m.Role
                }).ToListAsync();
        }

        public async Task<IEnumerable<BoardMemberReadDto>> GetByBoardIdAsync(Guid boardId)
        {
            return await _context.BoardMembers
                .Where(m => m.BoardId == boardId)
                .Select(m => new BoardMemberReadDto
                {
                    BoardId = m.BoardId,
                    UserId = m.UserId,
                    Role = m.Role
                }).ToListAsync();
        }

        public async Task<BoardMemberReadDto?> GetByIdsAsync(Guid boardId, Guid userId)
        {
            var member = await _context.BoardMembers.FirstOrDefaultAsync(m => m.BoardId == boardId && m.UserId == userId);
            if (member == null)
                return null;

            return new BoardMemberReadDto
            {
                BoardId = member.BoardId,
                UserId = member.UserId,
                Role = member.Role
            };
        }

        public async Task AddAsync(BoardMemberCreateDto dto)
        {
            bool exists = await _context.BoardMembers.AnyAsync(m =>
                m.BoardId == dto.BoardId && m.UserId == dto.UserId);
            if (exists)
                throw new ApiException("El miembro ya existe en el tablero.", 409);

            var newMember = new BoardMember
            {
                BoardId = dto.BoardId,
                UserId = dto.UserId,
                Role = dto.Role
            };

            _context.BoardMembers.Add(newMember);
            await _context.SaveChangesAsync();
        }

        public async Task AddManyAsync(List<BoardMemberCreateDto> dtos)
        {
            var newMembers = new List<BoardMember>();

            foreach (var dto in dtos)
            {
                bool exists = await _context.BoardMembers.AnyAsync(m =>
                    m.BoardId == dto.BoardId && m.UserId == dto.UserId);

                if (!exists)
                {
                    newMembers.Add(new BoardMember
                    {
                        BoardId = dto.BoardId,
                        UserId = dto.UserId,
                        Role = dto.Role
                    });
                }
            }

            _context.BoardMembers.AddRange(newMembers);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(BoardMemberUpdateDto dto)
        {
            var existing = await _context.BoardMembers.FirstOrDefaultAsync(m =>
                m.BoardId == dto.BoardId && m.UserId == dto.UserId);
            if (existing == null)
                throw new NotFoundException("Miembro no encontrado.");

            existing.Role = dto.Role;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid boardId, Guid userId)
        {
            var member = await _context.BoardMembers.FirstOrDefaultAsync(m =>
                m.BoardId == boardId && m.UserId == userId);
            if (member == null)
                throw new NotFoundException("Miembro no encontrado.");

            _context.BoardMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
