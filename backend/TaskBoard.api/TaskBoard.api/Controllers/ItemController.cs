using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskBoard.api.Data;
using TaskBoard.api.Services;
using TaskBoard.api.Models.Dtos;
using TaskBoard.api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/items")]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ActivityLogger _activityLogger;
        private readonly IHubContext<BoardHub> _boardHub;
        private readonly ItemService _itemService;
        private readonly BoardService _boardService;

        public ItemController(
         AppDbContext context,
         ActivityLogger activityLogger,
         IHubContext<BoardHub> boardHub,
         ItemService itemService,
         BoardService boardService)
        {
            _context = context;
            _activityLogger = activityLogger;
            _boardHub = boardHub;
            _itemService = itemService;
            _boardService = boardService;
        }

        [HttpPost("move")]
        public async Task<IActionResult> MoveItem([FromBody] ItemMoveDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


            if (!await _boardService.UserCanEditAsync(dto.BoardId, userId))
                return Forbid();

            var item = await _itemService.MoveItemAsync(dto.ItemId, dto.ToColumnId);


            await _boardHub.Clients.Group(dto.BoardId.ToString())
                .SendAsync("ItemMoved", new ItemMovedEventDto
                {
                    ItemId = item.Id,
                    FromColumnId = dto.FromColumnId,
                    ToColumnId = item.ColumnId,
                    Position = item.Order
                });

            return Ok(item);
        }
    }
}



