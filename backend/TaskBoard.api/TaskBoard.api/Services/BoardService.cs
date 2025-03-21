using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskBoard.api.Data;
using TaskBoard.api.Models;
using TaskBoard.api.Models.Dtos.Board;
using TaskBoard.api.Models.Dtos.BoardDtos;

namespace TaskBoard.api.Services
{
    public class BoardService
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
            return await _context.Boards
                .Include(b => b.Members)
                .Include(b => b.Columns)
                .ThenInclude(c => c.Items)
                .Where(b => b.Id == boardId &&
                    (b.OwnerId == userId || b.Members.Any(m => m.UserId == userId)))
                .Select(b => new BoardDetailDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsPublic = b.IsPublic,
                    Columns = _mapper.Map<List<ColumnDto>>(b.Columns),
                    Permissions = GetUserPermissions(b, userId)
                })
                .FirstOrDefaultAsync();
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
    }
}



