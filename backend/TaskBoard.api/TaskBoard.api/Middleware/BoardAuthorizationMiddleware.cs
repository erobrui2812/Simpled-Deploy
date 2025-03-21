using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskBoard.api.Data;
using TaskBoard.api.Models;

public class BoardAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public BoardAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, UserManager<User> userManager)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userGuid))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Usuario no autenticado");
            return;
        }

        var boardId = context.GetRouteValue("boardId")?.ToString();
        if (Guid.TryParse(boardId, out var boardGuid))
        {
            var hasAccess = await dbContext.BoardMembers
                .AnyAsync(m => m.BoardId == boardGuid && m.UserId == userGuid);

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


