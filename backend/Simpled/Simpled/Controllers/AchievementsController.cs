using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Achievements;

namespace Simpled.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController : ControllerBase
    {
        private readonly SimpledDbContext _context;

        public AchievementsController(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los logros de un usuario.
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<UnlockedAchievementDto>>> GetAchievementsForUser(Guid userId)
        {
            var logros = await _context.UserAchievements
                .Where(l => l.UserId == userId)
                .Select(l => new UnlockedAchievementDto
                {
                    Clave = $"{l.Accion}{l.Valor}",
                    FechaDesbloqueo = l.Fecha
                })
                .ToListAsync();

            return Ok(logros);
        }
    }
}