using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskBoard.api.Utils;
using Microsoft.Extensions.Options;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;

        public TestController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            Console.WriteLine($"Issuer: {_jwtSettings.Issuer}");
            Console.WriteLine($"Audience: {_jwtSettings.Audience}");
            Console.WriteLine($"Key: {_jwtSettings.Key}");
        }

        [HttpGet("auth")]
        [Authorize]
        public IActionResult GetWithAuth()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        [HttpGet("debug-token")]
        [AllowAnonymous]
        public IActionResult DebugToken([FromQuery] string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };

                handler.ValidateToken(token, validationParameters, out var validatedToken);
                return Ok("Token válido");
            }
            catch (Exception ex)
            {
                return BadRequest($"Token inválido: {ex.Message}");
            }
        }
    }
}
