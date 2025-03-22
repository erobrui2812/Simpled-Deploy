using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Data;
using TaskBoard.api.Models.Dtos.BoardDtos;

namespace TaskBoard.api.Hubs
{
    [Authorize]
    public class BoardHub : Hub
    {
        private readonly AppDbContext _context;

        public BoardHub(AppDbContext context) => _context = context;

        [Authorize(Policy = "BoardAccess")]
        public async Task JoinBoard(Guid boardId)
        {
            Console.WriteLine($"Intento de unirse al tablero: {boardId}");
            var userId = Guid.Parse(Context.UserIdentifier);
            var userRole = await _context.BoardMembers
                .Where(m => m.BoardId == boardId && m.UserId == userId)
                .Select(m => m.Role)
                .FirstOrDefaultAsync();

            if (userRole == null)
            {
                Console.WriteLine("Acceso denegado");
                await Clients.Caller.SendAsync("Error", "No tienes acceso a este tablero");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
            Console.WriteLine("Usuario añadido al grupo");
            await Clients.Caller.SendAsync("BoardJoined", $"Conectado como {userRole}");
        }

        [Authorize(Roles = "Editor,Admin")] // Solo editores y admins pueden mover ítems
        public async Task MoveItem(Guid boardId, ItemMovedEventDto eventData)
        {
            // Validar que el ítem pertenece al tablero
            var isValid = await _context.Items
                .AnyAsync(i => i.Id == eventData.ItemId && i.Column!.BoardId == boardId);

            if (!isValid)
                throw new HubException("Operación no válida");

            await Clients.Group(boardId.ToString())
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

