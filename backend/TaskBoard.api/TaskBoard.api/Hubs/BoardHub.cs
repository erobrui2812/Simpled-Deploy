using Microsoft.AspNetCore.SignalR;

namespace TaskBoard.api.Hubs
{
    public class BoardHub : Hub
    {
        public async Task JoinBoardGroup(string boardId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
        }

        public async Task NotifyCardMoved(string boardId, CardMovedEventData data)
        {
            await Clients.GroupExcept(boardId, Context.ConnectionId)
                .SendAsync("CardMoved", data);
        }
    }

    public class CardMovedEventData
    {
        public string CardId { get; set; }
        public string FromColumn { get; set; }
        public string ToColumn { get; set; }
        public int Position { get; set; }
    }
}

