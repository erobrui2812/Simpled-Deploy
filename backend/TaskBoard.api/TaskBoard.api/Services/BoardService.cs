using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskBoard.api.Data;
using TaskBoard.api.Models;
using TaskBoard.api.Models.Dtos.Board;
using TaskBoard.api.Models.Dtos.BoardDtos;

namespace TaskBoard.api.Services
{
    public class BoardService : IBoardService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BoardService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BoardResponseDto> CreateBoardAsync(BoardCreateDto dto, Guid userId)
        {
            var board = _mapper.Map<Board>(dto);
            board.OwnerId = userId;

            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            return _mapper.Map<BoardResponseDto>(board);
        }

        public async Task<BoardDetailDto> GetBoardWithPermissionsAsync(Guid boardId, Guid userId)
        {
            var board = await _context.Boards
                .Include(b => b.Members)
                .FirstOrDefaultAsync(b => b.Id == boardId);

            if (board == null || !board.Members.Any(m => m.UserId == userId))
                throw new UnauthorizedAccessException("No access to board");

            return _mapper.Map<BoardDetailDto>(board);
        }

        private BoardPermissionsDto GetUserPermissions(Board board, Guid userId)
        {
            var permissions = _mapper.Map<BoardPermissionsDto>(board);

            permissions.CanEdit = board.OwnerId == userId ||
                                 board.Members.Any(m => m.UserId == userId && m.Role == "Editor");
            permissions.CanDelete = board.OwnerId == userId;
            permissions.CanInvite = board.OwnerId == userId;

            return permissions;
        }

        public async Task<bool> UserCanEditAsync(Guid boardId, Guid userId)
        {
            return await _context.BoardMembers
                .AnyAsync(m => m.BoardId == boardId &&
                              m.UserId == userId &&
                              (m.Role == "Admin" || m.Role == "Editor"));
        }

        public async Task<List<BoardResponseDto>> GetBoardsByUserAsync(Guid userId)
        {
            var boards = await _context.Boards
                .Where(b => b.OwnerId == userId || b.Members.Any(m => m.UserId == userId))
                .ToListAsync();
            return _mapper.Map<List<BoardResponseDto>>(boards);
        }

    }
}

