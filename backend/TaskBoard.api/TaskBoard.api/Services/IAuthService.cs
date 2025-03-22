using System.Security.Claims;
using TaskBoard.api.Models;

namespace TaskBoard.api.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user, IList<string> roles);
        ClaimsPrincipal ValidateJwtToken(string token);
    }
}
