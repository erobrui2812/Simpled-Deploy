using Simpled.Dtos.Chat;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestión de salas de chat y sus mensajes.
    /// </summary>
    public interface IChatRepository
    {
        /// <summary>
        /// Obtiene (o crea si no existe) la sala de chat para un Team o Board.
        /// </summary>
        /// <param name="roomType">"Team" o "Board".</param>
        /// <param name="entityId">ID del Team o Board.</param>
        /// <returns>DTO de la sala.</returns>
        Task<ChatRoomReadDto> GetOrCreateRoomAsync(string roomType, Guid entityId);

        /// <summary>
        /// Recupera todos los mensajes de una sala de chat, ordenados por fecha ascendente.
        /// </summary>
        /// <param name="chatRoomId">ID de la sala.</param>
        Task<IEnumerable<ChatMessageReadDto>> GetMessagesAsync(Guid chatRoomId);

        /// <summary>
        /// Añade un nuevo mensaje a la sala.
        /// </summary>
        /// <param name="userId">ID del usuario que envía el mensaje.</param>
        /// <param name="dto">Datos del mensaje a crear.</param>
        /// <returns>DTO del mensaje creado.</returns>
        Task<ChatMessageReadDto> AddMessageAsync(Guid userId, ChatMessageCreateDto dto);
    }
}
