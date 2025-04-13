using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Simpled.Hubs
{
    public class BoardHub : Hub
    {
        // Notifica a un usuario específico 
        public async Task SendInvitationNotification(string email, string message)
        {
            await Clients.User(email).SendAsync("InvitationReceived", message);
        }

        // Notifica a todos los usuarios conectados a un tablero
        public async Task NotifyBoardUpdated(Guid boardId)
        {
            await Clients.Group(boardId.ToString()).SendAsync("BoardUpdated", boardId);
        }

        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, email.ToLower());

            }

            await base.OnConnectedAsync();
        }
    }
}
