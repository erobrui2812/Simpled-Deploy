using System.Security.Claims;

namespace TaskBoard.api.Utils;

public static class UserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Usuario no autenticado");

        return Guid.Parse(userId);
    }
}