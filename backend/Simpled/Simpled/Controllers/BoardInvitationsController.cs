using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.BoardInvitations;
using Simpled.Helpers;
using Simpled.Repository;
using System.Security.Claims;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardInvitationsController : ControllerBase
    {
        private readonly IBoardInvitationRepository _invitationService;
        private readonly IBoardMemberRepository _boardMemberRepo;

        public BoardInvitationsController(
            IBoardInvitationRepository invitationService,
            IBoardMemberRepository boardMemberRepo)
        {
            _invitationService = invitationService;
            _boardMemberRepo = boardMemberRepo;
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
        [HttpGet("{token}")]
        public async Task<IActionResult> GetByToken(string token)
        {
            var result = await _invitationService.GetByTokenAsync(token);
            return result == null ? NotFound("Invitación no encontrada.") : Ok(result);
        }

        /// <summary>
        /// Crea una nueva invitación a un tablero (solo admin del board).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BoardInvitationCreateDto dto)
        {
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, dto.BoardId, new[] { "admin" }, _boardMemberRepo);

            if (!hasPermission)
                return Forbid("No tienes permisos para invitar usuarios a este tablero.");

            await _invitationService.CreateAsync(dto);
            return Ok("Invitación enviada.");
        }

        /// <summary>
        /// Acepta una invitación usando el token.
        /// </summary>
        [HttpPost("accept")]
        public async Task<IActionResult> Accept([FromBody] BoardInvitationAcceptDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _invitationService.AcceptAsync(dto.Token, Guid.Parse(userId));
            return success ? Ok("Invitación aceptada.") : BadRequest("No se pudo aceptar la invitación.");
        }

        /// <summary>
        /// Rechaza una invitación usando el token.
        /// </summary>
        [HttpPost("reject")]
        public async Task<IActionResult> Reject([FromBody] BoardInvitationAcceptDto dto)
        {
            var success = await _invitationService.RejectAsync(dto.Token);
            return success ? Ok("Invitación rechazada.") : NotFound("Invitación no encontrada.");
        }
    }
}
