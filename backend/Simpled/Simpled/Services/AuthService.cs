using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Simpled.Data;
using Simpled.Dtos.Auth;
using Simpled.Models;
using Simpled.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Simpled.Services
{
    public class AuthService : IAuthRepository
    {
        private readonly SimpledDbContext _context;
        private readonly AchievementsService _achievementsService;
        private readonly IConfiguration _configuration;

        public AuthService(SimpledDbContext context, AchievementsService achievementsService, IConfiguration configuration)
        {
            _context = context;
            _achievementsService = achievementsService;
            _configuration = configuration;
        }

        public async Task<LoginResultDto?> LoginAsync(LoginRequestDto loginDto)

        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", user.Email)
            };

            foreach (var role in user.Roles.Select(r => r.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            // TESTING: logros automáticos al iniciar sesión
            user.createdBoardsCount = 10;
            user.createdTasksCount = 50;
            user.completedTasksCount = 5;
            user.teamsCount = 3;
            await _context.SaveChangesAsync();

            var logrosTesting = new List<string>();
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "CrearTablero", user.createdBoardsCount));
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "CrearTarea", user.createdTasksCount));
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "CompletarTarea", user.completedTasksCount));
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "UnirseEquipo", user.teamsCount));

            foreach (var logro in logrosTesting)
            {
                Console.WriteLine($" Logro desbloqueado al logear: {logro}");
            }

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResultDto
            {
                Token = tokenString,
                UserId = user.Id.ToString()
            };
        }
    }
}
