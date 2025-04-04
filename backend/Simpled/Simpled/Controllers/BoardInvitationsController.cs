using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.BoardInvitations;
using Simpled.Helpers;
using Simpled.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Simpled.Hubs;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardInvitationsController : ControllerBase
    {
        private readonly IBoardInvitationRepository _invitationService;
        private readonly IBoardMemberRepository _boardMemberRepo;
        private readonly IHubContext<BoardHub> _hub;

        public BoardInvitationsController(
            IBoardInvitationRepository invitationService,
            IBoardMemberRepository boardMemberRepo,
            IHubContext<BoardHub> hub)
        {
            _invitationService = invitationService;
            _boardMemberRepo = boardMemberRepo;
            _hub = hub;
        }

        /// <summary>
        /// Obtiene todas las invitaciones pendientes para el usuario actual.
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetUserInvitations()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("No se pudo obtener el correo del usuario.");

            var result = await _invitationService.GetAllByEmailAsync(email.ToLower());
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una invitación por token.
        /// </summary>
        /// <param name="token">Token de la invitación</param>
        [HttpGet("{token}")]
        public async Task<IActionResult> GetByToken(string token)
        {
            var result = await _invitationService.GetByTokenAsync(token);
            return result == null ? NotFound("Invitación no encontrada.") : Ok(result);
        }

        /// <summary>
        /// Crea una nueva invitación a un tablero (solo admin del board).
        /// </summary>
        /// <param name="dto">Datos de la invitación a crear</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BoardInvitationCreateDto dto)
        {
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, dto.BoardId, new[] { "admin" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos para invitar usuarios a este tablero.");

            await _invitationService.CreateAsync(dto);
            await _hub.Clients.User(dto.Email).SendAsync(
                "InvitationReceived",
                $"Has sido invitado al tablero con rol {dto.Role}");

            return Ok("Invitación enviada.");
        }

        /// <summary>
        /// Acepta una invitación usando el token.
        /// </summary>
        /// <param name="dto">Datos de la invitación a aceptar</param>
        [HttpPost("accept")]
        public async Task<IActionResult> Accept([FromBody] BoardInvitationAcceptDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _invitationService.AcceptAsync(dto.Token, Guid.Parse(userId));
            if (success)
            {
                var boardId = (await _invitationService.GetByTokenAsync(dto.Token))?.BoardId;
                if (boardId != null)
                {
                    await _hub.Clients.Group(boardId.ToString()!).SendAsync("BoardUpdated", boardId);
                }
            }

            return success ? Ok("Invitación aceptada.") : BadRequest("No se pudo aceptar la invitación.");
        }

        /// <summary>
        /// Rechaza una invitación usando el token.
        /// </summary>
        /// <param name="dto">Datos de la invitación a rechazar</param>
        [HttpPost("reject")]
        public async Task<IActionResult> Reject([FromBody] BoardInvitationAcceptDto dto)
        {
            var success = await _invitationService.RejectAsync(dto.Token);
            return success ? Ok("Invitación rechazada.") : NotFound("Invitación no encontrada.");
        }
    }
}
