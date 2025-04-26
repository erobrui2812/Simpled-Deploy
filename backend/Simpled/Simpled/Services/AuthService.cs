using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Simpled.Data;
using Simpled.Dtos.Auth;
using Simpled.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Simpled.Models;
using System;





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
            user.CreatedBoardsCount = 10;
            user.CreatedTasksCount = 50;
            user.CompletedTasksCount = 5;
            user.TeamsCount = 3;
            await _context.SaveChangesAsync();

            var logrosTesting = new List<string>();
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "CrearTablero", user.CreatedBoardsCount));
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "CrearTarea", user.CreatedTasksCount));
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "CompletarTarea", user.CompletedTasksCount));
            logrosTesting.AddRange(await _achievementsService.ProcessActionAsync(user, "UnirseEquipo", user.TeamsCount));

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

        private string GenerateConfirmationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }


        public async Task<ConfirmEmailResultDto> ConfirmEmailAsync(ConfirmEmailDto confirmDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == confirmDto.Email);

            if (user == null)
            {
                return new ConfirmEmailResultDto
                {
                    Success = false,
                    ErrorMessage = "Usuario no encontrado."
                };
            }

            if (user.IsEmailConfirmed)
            {
                return new ConfirmEmailResultDto
                {
                    Success = false,
                    ErrorMessage = "El correo ya fue confirmado anteriormente."
                };
            }

            if (user.ConfirmationCode != confirmDto.Code)
            {
                return new ConfirmEmailResultDto
                {
                    Success = false,
                    ErrorMessage = "Código de confirmación incorrecto."
                };
            }

            user.IsEmailConfirmed = true;
            user.ConfirmationCode = null; // Limpia el código después de confirmar

            await _context.SaveChangesAsync();

            return new ConfirmEmailResultDto
            {
                Success = true
            };
        }



        private Task SendConfirmationEmail(string email, string confirmationCode)
        {
            Console.WriteLine($"Enviar email a {email} con el código de verificación: {confirmationCode}");
            return Task.CompletedTask;
        }


        public async Task<GoogleLoginResultDto> LoginWithGoogleAsync(GoogleLoginDto googleLoginDto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken);

                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (user == null)
                {
                    // Usuario nuevo
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Name = payload.Name,
                        Email = payload.Email,
                        CreatedAt = DateTime.UtcNow,
                        PasswordHash = "", // No tiene contraseña
                        ImageUrl = payload.Picture ?? "/images/default/avatar-default.jpg",
                        Roles = new List<UserRole> { new UserRole { Role = "viewer" } },
                        IsEmailConfirmed = false,
                        ConfirmationCode = GenerateConfirmationCode()
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await SendConfirmationEmail(user.Email, user.ConfirmationCode);
                }

                if (!user.IsEmailConfirmed)
                {
                    return new GoogleLoginResultDto
                    {
                        Success = true,
                        NeedsVerification = true,
                        UserId = user.Id.ToString(),
                        Token = ""
                    };
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
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

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return new GoogleLoginResultDto
                {
                    Success = true,
                    Token = tokenString,
                    UserId = user.Id.ToString(),
                    NeedsVerification = false
                };
            }
            catch (Exception ex)
            {
                return new GoogleLoginResultDto
                {
                    Success = false,
                    ErrorMessage = $"Error validando token de Google: {ex.Message}"
                };
            }
        }
    }
}
