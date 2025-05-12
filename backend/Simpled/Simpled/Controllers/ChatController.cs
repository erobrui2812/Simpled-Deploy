using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Chat;
using Simpled.Repository;

namespace Simpled.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepo;

        /// <summary>
        /// Constructor que inyecta el repositorio de chat.
        /// </summary>
        public ChatController(IChatRepository chatRepo)
        {
            _chatRepo = chatRepo;
        }

        /// <summary>
        /// Obtiene o crea la sala de chat para un Team o Board.
        /// </summary>
        /// <param name="roomType">Tipo de sala: "Team" o "Board".</param>
        /// <param name="entityId">ID del Team o Board.</param>
        /// <returns>DTO con los datos de la sala de chat.</returns>
        [HttpGet("rooms/{roomType}/{entityId:guid}")]
        [ProducesResponseType(typeof(ChatRoomReadDto), 200)]
        public async Task<IActionResult> GetOrCreateRoom(string roomType, Guid entityId)
        {
            // Validar roomType
            if (!string.Equals(roomType, "Team", StringComparison.OrdinalIgnoreCase)
             && !string.Equals(roomType, "Board", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("El tipo de sala debe ser 'Team' o 'Board'.");
            }

            var room = await _chatRepo.GetOrCreateRoomAsync(roomType, entityId);
            return Ok(room);
        }

        /// <summary>
        /// Recupera todos los mensajes de una sala de chat, ordenados cronológicamente.
        /// </summary>
        /// <param name="roomId">ID de la sala de chat.</param>
        [HttpGet("rooms/{roomId:guid}/messages")]
        [ProducesResponseType(typeof(ChatMessageReadDto[]), 200)]
        public async Task<IActionResult> GetMessages(Guid roomId)
        {
            var messages = await _chatRepo.GetMessagesAsync(roomId);
            return Ok(messages);
        }

        /// <summary>
        /// Añade un nuevo mensaje a la sala de chat.
        /// </summary>
        /// <param name="dto">DTO con ChatRoomId y texto del mensaje.</param>
        [HttpPost("messages")]
        [ProducesResponseType(typeof(ChatMessageReadDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddMessage([FromBody] ChatMessageCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("No se pudo identificar al usuario.");

            var userId = Guid.Parse(userIdClaim);

            var created = await _chatRepo.AddMessageAsync(userId, dto);
            return CreatedAtAction(
                nameof(GetMessages),
                new { roomId = created.ChatRoomId },
                created
            );
        }
    }
}
