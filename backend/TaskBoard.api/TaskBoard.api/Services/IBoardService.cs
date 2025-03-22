using TaskBoard.api.Models.Dtos.BoardDtos;
using TaskBoard.api.Models.Dtos.Board;

namespace TaskBoard.api.Services;

public interface IBoardService
{
    Task<BoardResponseDto> CreateBoardAsync(BoardCreateDto dto, Guid userId);
    Task<BoardDetailDto> GetBoardWithPermissionsAsync(Guid boardId, Guid userId);
    Task<bool> UserCanEditAsync(Guid boardId, Guid userId);
    Task<List<BoardResponseDto>> GetBoardsByUserAsync(Guid userId);

}