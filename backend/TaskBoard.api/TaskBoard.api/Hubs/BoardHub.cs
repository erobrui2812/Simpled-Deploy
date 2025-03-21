using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Data;
using TaskBoard.api.Models.Dtos.BoardDtos;

namespace TaskBoard.api.Hubs
{
    public class BoardHub : Hub
    {
        private readonly AppDbContext _context;

        public BoardHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task JoinBoard(Guid boardId)
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var hasAccess = await _context.Boards
                .AnyAsync(b => b.Id == boardId &&
                    (b.OwnerId == userId || b.Members.Any(m => m.UserId == userId)));

            if (hasAccess)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
                await Clients.Caller.SendAsync("BoardJoined", $"Conectado al tablero {boardId}");
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Acceso denegado");
            }
        }

        public async Task MoveItem(Guid boardId, ItemMovedEventDto eventData)
        {
            // Validar permiso de edición
            await Clients.GroupExcept(boardId.ToString(), Context.ConnectionId)
                .SendAsync("ItemMoved", eventData);
        }
    }

    public class ItemMovedEventDto
    {
        public Guid ItemId { get; set; }
        public Guid FromColumnId { get; set; }
        public Guid ToColumnId { get; set; }
        public int Position { get; set; }
    }
}



