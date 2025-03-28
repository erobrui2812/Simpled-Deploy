using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Boards;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    public class BoardService : IBoardRepository
    {
        private readonly SimpledDbContext _context;

        public BoardService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoardReadDto>> GetAllAsync()
        {
            return await _context.Boards
                .Select(b => new BoardReadDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    OwnerId = b.OwnerId,
                    IsPublic = b.IsPublic
                })
                .ToListAsync();
        }

        public async Task<BoardReadDto?> GetByIdAsync(Guid id)
        {
            var b = await _context.Boards.FindAsync(id);
            if (b == null) return null;

            return new BoardReadDto
            {
                Id = b.Id,
                Name = b.Name,
                OwnerId = b.OwnerId,
                IsPublic = b.IsPublic
            };
        }

        public async Task<BoardReadDto> CreateAsync(BoardCreateDto dto)
        {
            var newBoard = new Board
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                OwnerId = dto.OwnerId,
                IsPublic = dto.IsPublic
            };

            _context.Boards.Add(newBoard);
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
            if (board == null) return false;

            board.Name = dto.Name;
            board.OwnerId = dto.OwnerId;
            board.IsPublic = dto.IsPublic;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var board = await _context.Boards.FindAsync(id);
            if (board == null) return false;

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
