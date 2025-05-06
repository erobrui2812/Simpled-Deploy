using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Users;
using Simpled.Models;

namespace Simpled.Controllers
{
    [ApiController]
    [Route("api/favorite-boards")]
    [Authorize]
    public class FavoriteBoardsController : ControllerBase
    {
        private readonly SimpledDbContext _context;

        public FavoriteBoardsController(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Modifica el estado de favorito de un tablero de true a false 
        /// </summary>
        /// <param name="dto">Id del tablero</param>
        /// <returns>True de si es favorito o no</returns>
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteBoardDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = string.IsNullOrEmpty(userIdClaim) ? null : Guid.Parse(userIdClaim);
            if (userId == null) return Unauthorized();


            var favorite = await _context.FavoriteBoards.FindAsync(userId, dto.BoardId);

            if (favorite != null)
            {
                _context.FavoriteBoards.Remove(favorite);
                await _context.SaveChangesAsync();
                return Ok(new { favorite = false });
            }

            _context.FavoriteBoards.Add(new FavoriteBoards
            {
                UserId = (Guid) userId,
                BoardId = dto.BoardId
            });

            await _context.SaveChangesAsync();
            return Ok(new { favorite = true });
        }

        /// <summary>
        /// Revisa si un tablero es favorito o no
        /// </summary>
        /// <param name="dto">Id del tablero</param>
        /// <returns>True de si es favorito o no</returns>
        [HttpGet("check-favorite/{boardId}")]
        public async Task<IActionResult> CheckFavorite(FavoriteBoardDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = string.IsNullOrEmpty(userIdClaim) ? null : Guid.Parse(userIdClaim);
            if (userId == null) return Unauthorized();

            var exists = await _context.FavoriteBoards.AnyAsync(f =>
                f.UserId == userId && f.BoardId == dto.BoardId);

            return Ok(new { favorite = exists });
        }

        /// <summary>
        /// Hace una lista con los tableros que tengas marcados como favoritos
        /// </summary>
        /// <returns>True de si es favorito o no</returns>
        [HttpGet]
        public async Task<IActionResult> GetFavoriteBoardNames()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = string.IsNullOrEmpty(userIdClaim) ? null : Guid.Parse(userIdClaim);
            if (userId == null) return Unauthorized();

            var list = await _context.FavoriteBoards
                .Where(f => f.UserId == userId)
                .Include(f => f.Board)
                .Select(f => new {
                    Id = f.Board!.Id,
                    Name = f.Board!.Name
                })
                .ToListAsync();


            return Ok(list);
        }
    }
}
