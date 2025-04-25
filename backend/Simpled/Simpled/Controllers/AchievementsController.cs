using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Achievements;
using Simpled.Services;

namespace Simpled.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController : ControllerBase
    {
        private readonly SimpledDbContext _context;
        private readonly AchievementsService _achievementsService;

        public AchievementsController(SimpledDbContext context, AchievementsService achievementsService)
        {
            _context = context;
            _achievementsService = achievementsService;
        }

        /// <summary>
        /// Obtiene todos los logros de un usuario.
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<UnlockedAchievementDto>>> GetAchievementsForUser(Guid userId)
        {
            var achievements = await _context.UserAchievements
                .Where(l => l.UserId == userId)
                .Select(l => new UnlockedAchievementDto
                {
                    Key = $"{l.Action}{l.Value}",
                    unlockedDate = l.Date
                })
                .ToListAsync();

            return Ok(achievements);
        }

        /// <summary>
        /// Obtiene todos los logros de un usuario.
        /// </summary>
        [HttpGet("achievements")]
        public ActionResult<List<object>> GetExistingAchievements()
        {
            var achievements = _achievementsService.GetAllAchievements();
            return Ok(achievements);
        }
    }
}