using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Chat;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión de salas de chat y mensajes.
    /// Implementa IChatRepository.
    /// </summary>
    public class ChatService : IChatRepository
    {
        private readonly SimpledDbContext _context;

        public ChatService(SimpledDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<ChatRoomReadDto> GetOrCreateRoomAsync(string roomType, Guid entityId)
        {
 
            var room = await _context.ChatRooms
                .FirstOrDefaultAsync(r => r.RoomType == roomType && r.EntityId == entityId);

            if (room == null)
            {
                // Si no existe, la creamos
                room = new ChatRoom
                {
                    Id = Guid.NewGuid(),
                    RoomType = roomType,
                    EntityId = entityId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ChatRooms.Add(room);
                await _context.SaveChangesAsync();
            }

            return new ChatRoomReadDto
            {
                Id = room.Id,
                RoomType = room.RoomType,
                EntityId = room.EntityId
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ChatMessageReadDto>> GetMessagesAsync(Guid chatRoomId)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageReadDto
                {
                    Id = m.Id,
                    ChatRoomId = m.ChatRoomId,
                    UserId = m.UserId,
                    Text = m.Text,
                    SentAt = m.SentAt
                })
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<ChatMessageReadDto> AddMessageAsync(Guid userId, ChatMessageCreateDto dto)
        {
            // Verificamos que la sala exista
            var roomExists = await _context.ChatRooms
                .AnyAsync(r => r.Id == dto.ChatRoomId);
            if (!roomExists)
                throw new KeyNotFoundException($"ChatRoom {dto.ChatRoomId} no encontrada.");

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatRoomId = dto.ChatRoomId,
                UserId = userId,
                Text = dto.Text,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return new ChatMessageReadDto
            {
                Id = message.Id,
                ChatRoomId = message.ChatRoomId,
                UserId = message.UserId,
                Text = message.Text,
                SentAt = message.SentAt
            };
        }
    }
}
