using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Data;
using TaskBoard.api.Models;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/boards")]
    [Authorize]
    public class BoardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BoardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserBoards()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var boards = await _context.Boards
                .Where(b => b.OwnerId == userId || b.Members.Any(m => m.UserId == userId))
                .ToListAsync();

            return Ok(boards);
        }
    }
}

