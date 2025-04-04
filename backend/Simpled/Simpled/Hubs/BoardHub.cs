using Microsoft.AspNetCore.SignalR;

namespace Simpled.Hubs
{
    public class BoardHub : Hub
    {
        // Notifica a un usuario específico (por email)
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
            var userEmail = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userEmail))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userEmail); // para notificar por email
            }

            await base.OnConnectedAsync();
        }
    }
}
