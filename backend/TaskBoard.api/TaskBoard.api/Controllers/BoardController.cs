using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TaskBoard.api.Hubs;
using TaskBoard.api.Models.Dtos.Board;
using TaskBoard.api.Models.Dtos.BoardDtos;
using TaskBoard.api.Models;
using TaskBoard.api.Services;

[ApiController]
[Route("api/boards")]
[Authorize]
public class BoardController : ControllerBase
{
    private readonly BoardService _boardService;
    private readonly IHubContext<BoardHub> _boardHub;

    public BoardController(
        BoardService boardService,
        IHubContext<BoardHub> boardHub)
    {
        _boardService = boardService;
        _boardHub = boardHub;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoard([FromBody] BoardCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var board = await _boardService.CreateBoardAsync(dto, userId);

        await _boardHub.Clients.Group(board.Id.ToString())
            .SendAsync("BoardCreated", board);

        return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
    }

    [HttpGet("{boardId}")]
    public async Task<ActionResult<BoardDetailDto>> GetBoard(Guid boardId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return await _boardService.GetBoardWithPermissionsAsync(boardId, userId);
    }
}


