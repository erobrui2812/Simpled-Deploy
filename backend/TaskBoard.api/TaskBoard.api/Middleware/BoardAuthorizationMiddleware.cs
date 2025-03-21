using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskBoard.api.Data;
using TaskBoard.api.Models;

namespace TaskBoard.api.Middleware
{
    public class BoardAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public BoardAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            AppDbContext dbContext,
            UserManager<User> userManager)
        {
            var userId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var boardId = context.GetRouteValue("boardId")?.ToString();

            if (!string.IsNullOrEmpty(boardId) && Guid.TryParse(boardId, out var guidBoardId))
            {
                var hasAccess = await dbContext.Boards
                    .AnyAsync(b => b.Id == guidBoardId &&
                        (b.OwnerId == userId || b.Members.Any(m => m.UserId == userId)));

                if (!hasAccess)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Acceso denegado al tablero");
                    return;
                }
            }

            await _next(context);
        }
    }
}


