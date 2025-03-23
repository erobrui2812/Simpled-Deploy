using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Simpled.Models;
using Simpled.Data;
using Simpled.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Simpled.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SimpledDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(SimpledDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginDto)
        {

            var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);
            if (user == null)
                return Unauthorized("Email not found.");

            // Verificar pass con BCrypt
            bool validPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!validPassword)
                return Unauthorized("Invalid password.");

            // Claims
            var roles = user.Roles.Select(r => r.Role).ToList();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            foreach (var role in roles)
            {
                // role = "admin", "editor", etc.
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Generar JWT
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { token = jwt });
        }
    }
}
