using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Boards;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exceptions;

namespace Simpled.Services
{
    public class BoardService : IBoardRepository
    {
        private readonly SimpledDbContext _context;

        public BoardService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoardReadDto>> GetAllAsync(Guid? userId = null)
        {
            var boards = await _context.Boards
                .Where(b =>
                    b.IsPublic ||
                    (userId != null &&
                     (b.OwnerId == userId ||
                      _context.BoardMembers.Any(m => m.BoardId == b.Id && m.UserId == userId))))
                .Select(b => new BoardReadDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    OwnerId = b.OwnerId,
                    IsPublic = b.IsPublic,
                    UserRole = userId != null
                        ? _context.BoardMembers
                            .Where(m => m.BoardId == b.Id && m.UserId == userId)
                            .Select(m => m.Role)
                            .FirstOrDefault()
                        : null
                })
                .ToListAsync();

            return boards;
        }


        public async Task<BoardReadDto?> GetByIdAsync(Guid id)
        {
            var b = await _context.Boards.FindAsync(id);
            if (b == null)
                throw new NotFoundException("Tablero no encontrado.");

            return new BoardReadDto
            {
                Id = b.Id,
                Name = b.Name,
                OwnerId = b.OwnerId,
                IsPublic = b.IsPublic
            };
        }

        public async Task<BoardReadDto> CreateAsync(BoardCreateDto dto, Guid userId)
        {
            var newBoard = new Board
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                OwnerId = userId,
                IsPublic = dto.IsPublic
            };

            _context.Boards.Add(newBoard);

            _context.BoardMembers.Add(new BoardMember
            {
                BoardId = newBoard.Id,
                UserId = userId,
                Role = "admin"
            });

            await _context.SaveChangesAsync();

            return new BoardReadDto
            {
                Id = newBoard.Id,
                Name = newBoard.Name,
                OwnerId = newBoard.OwnerId,
                IsPublic = newBoard.IsPublic
            };
        }

        public async Task<bool> UpdateAsync(BoardUpdateDto dto)
        {
            var board = await _context.Boards.FindAsync(dto.Id);
            if (board == null)
                throw new NotFoundException("Tablero no encontrado.");

            board.Name = dto.Name;
            board.IsPublic = dto.IsPublic;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var board = await _context.Boards.FindAsync(id);
            if (board == null)
                throw new NotFoundException("Tablero no encontrado.");

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Board?> GetBoardByIdAsync(Guid id)
        {
            return await _context.Boards
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
