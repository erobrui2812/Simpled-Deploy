using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TaskBoard.api.Hubs;
using TaskBoard.api.Models.Dtos.Board;
using TaskBoard.api.Models.Dtos.BoardDtos;
using TaskBoard.api.Services;
using TaskBoard.api.Filters;
using TaskBoard.api.Utils;

[ApiController]
[Route("api/boards")]
[Authorize]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IHubContext<BoardHub> _boardHub;

    public BoardController(
        IBoardService boardService,
        IHubContext<BoardHub> boardHub)
    {
        _boardService = boardService;
        _boardHub = boardHub;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> CreateBoard([FromBody] BoardCreateDto dto)
    {
        var userId = User.GetUserId();
        var board = await _boardService.CreateBoardAsync(dto, userId);

        await _boardHub.Clients.Group(board.Id.ToString())
            .SendAsync("BoardCreated", board);

        return CreatedAtAction(nameof(GetBoard), new { boardId = board.Id }, board);
    }

    [HttpGet("{boardId:guid}")]
    [ServiceFilter(typeof(BoardAccessFilter))]
    public async Task<ActionResult<BoardDetailDto>> GetBoard(Guid boardId)
    {
        var board = await _boardService.GetBoardWithPermissionsAsync(
            boardId,
            User.GetUserId()
        );
        return Ok(board);
    }

    [HttpGet]
    public async Task<IActionResult> GetBoards()
    {
        var userId = User.GetUserId();
        var boards = await _boardService.GetBoardsByUserAsync(userId);
        return Ok(boards);
    }


}
