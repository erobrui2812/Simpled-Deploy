using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Simpled.Dtos.Chat;
using Simpled.Repository;

namespace Simpled.Hubs
{
    /// <summary>
    /// Hub de SignalR para gestionar chats de equipo y de tablero.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepo;


        public ChatHub(IChatRepository chatRepo)
        {
            _chatRepo = chatRepo;
        }

        /// <summary>
        /// Obtiene el ID del usuario conectado a partir de sus claims.
        /// </summary>
        private Guid CurrentUserId =>
            Guid.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        /// <summary>
        /// Añade la conexión actual al grupo de chat de un Team.
        /// </summary>
        /// <param name="teamId">Identificador del equipo.</param>
        public async Task JoinTeamRoom(Guid teamId)
        {
            // Creamos o recuperamos la sala de chat
            var room = await _chatRepo.GetOrCreateRoomAsync("Team", teamId);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
        }

        /// <summary>
        /// Añade la conexión actual al grupo de chat de un Board.
        /// </summary>
        /// <param name="boardId">Identificador del tablero.</param>
        public async Task JoinBoardRoom(Guid boardId)
        {
            var room = await _chatRepo.GetOrCreateRoomAsync("Board", boardId);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
        }

        /// <summary>
        /// Elimina la conexión actual del grupo de chat especificado.
        /// </summary>
        /// <param name="chatRoomId">Identificador de la sala de chat.</param>
        public async Task LeaveRoom(Guid chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        }

        /// <summary>
        /// Envía un mensaje a la sala de chat indicada:
        /// 1. Guarda el mensaje en la base de datos.
        /// 2. Retransmite el mensaje a todos los miembros del grupo.
        /// </summary>
        /// <param name="dto">DTO con ChatRoomId y texto.</param>
        public async Task SendMessage(ChatMessageCreateDto dto)
        {
            // 1. Persistir en BBDD
            var created = await _chatRepo.AddMessageAsync(CurrentUserId, dto);

            // 2. Retransmitir
            await Clients
                .Group(created.ChatRoomId.ToString())
                .SendAsync("ReceiveMessage", created);
        }

        /// <summary>
        /// Cuando un cliente se conecta, puede usar este método para unirse
        /// automáticamente a sus chats activos si lo deseas (opcional).
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
