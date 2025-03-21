using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Data;
using TaskBoard.api.Models; // Añadir este using
using TaskBoard.api.Models.Dtos.BoardDtos;
using TaskBoard.api.Services;
using System.Text.Json;
using TaskBoard.api.Models.Dtos;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/items")]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ActivityLogger _activityLogger;

        public ItemController(AppDbContext context, ActivityLogger activityLogger)
        {
            _context = context;
            _activityLogger = activityLogger;
        }

        [HttpPost("move")]
        public async Task<IActionResult> MoveItem([FromBody] ItemMoveDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // 1. Validar permisos
            var hasAccess = await _context.BoardMembers
                .AnyAsync(m => m.BoardId == dto.BoardId && m.UserId == userId);

            if (!hasAccess) return Forbid();

            // 2. Actualizar posición
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == dto.ItemId);

            if (item == null) return NotFound("Item no encontrado");

            item.ColumnId = dto.ToColumnId;
            await _context.SaveChangesAsync();

            // 3. Registrar actividad
            await _activityLogger.LogActivity(
                dto.BoardId,
                userId,
                "ItemMoved",
                "Item",
                new
                {
                    FromColumn = dto.FromColumnId,
                    ToColumn = dto.ToColumnId,
                    ItemId = dto.ItemId
                });

            return Ok(item);
        }
    }
}
