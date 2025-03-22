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
        // Ignorar rutas que no requieren autorización
        if (context.Request.Path.StartsWithSegments("/api/boards") && context.Request.Method == "POST")
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path;

        // Ignorar middleware en rutas públicas (Auth)
        if (path.StartsWithSegments("/api/Auth"))
        {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userGuid))
        {
            // Solo requerir autenticación si la ruta tiene boardId
            var boardId = context.GetRouteValue("boardId")?.ToString();
            if (!string.IsNullOrEmpty(boardId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Usuario no autenticado");
                return;
            }
        }

        var boardIdValue = context.GetRouteValue("boardId")?.ToString();
        if (Guid.TryParse(boardIdValue, out var boardGuid))
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




