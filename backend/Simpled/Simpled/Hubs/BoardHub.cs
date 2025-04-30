using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Simpled.Hubs
{
    public class BoardHub : Hub
    {
        /// <summary>
        /// Notifica a un usuario de una invitación de tablero.
        /// </summary>
        public async Task SendInvitationNotification(string email, string message)
        {
            await Clients.User(email).SendAsync("InvitationReceived", message);
        }

        /// <summary>
        /// Notifica a un usuario de una invitación de equipo.
        /// </summary>
        public async Task SendTeamInvitation(string email, string teamName, string role)
        {
            await Clients.User(email).SendAsync("TeamInvitationReceived", new
            {
                teamName,
                role
            });
        }

        /// <summary>
        /// Notifica a todos los usuarios de un tablero actualizado.
        /// </summary>
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
