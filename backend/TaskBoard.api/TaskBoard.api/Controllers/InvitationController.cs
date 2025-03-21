using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Services;
using TaskBoard.api.Models.Dtos;
using TaskBoard.api.Data;
using TaskBoard.api.Models;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    [Authorize]
    public class InvitationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly InvitationService _invitationService;

        public InvitationController(AppDbContext context, InvitationService invitationService)
        {
            _context = context;
            _invitationService = invitationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvitation([FromBody] InvitationCreateDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var invitation = await _invitationService.CreateInvitation(dto, userId);
            return Ok(invitation);
        }

        [HttpPost("join/{code}")]
        public async Task<IActionResult> JoinBoardByCode(string code)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.Code == code && i.Expiration > DateTime.UtcNow);

            if (invitation == null) return BadRequest("Invalid or expired code");

            var existingMember = await _context.BoardMembers
                .AnyAsync(m => m.BoardId == invitation.BoardId && m.UserId == userId);

            if (!existingMember)
            {
                _context.BoardMembers.Add(new BoardMember
                {
                    BoardId = invitation.BoardId,
                    UserId = userId,
                    Role = invitation.Role
                });

                await _context.SaveChangesAsync();
            }

            return Ok(new { BoardId = invitation.BoardId });
        }
    }
}



