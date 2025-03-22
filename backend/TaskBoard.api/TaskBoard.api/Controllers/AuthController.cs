using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TaskBoard.api.Models;
using TaskBoard.api.Models.Dtos.AuthDtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using TaskBoard.api.Services;
using TaskBoard.api.Utils;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public AuthController(
            UserManager<User> userManager,
            IOptions<JwtSettings> jwtSettings,
            RoleManager<IdentityRole<Guid>> roleManager,
            IMapper mapper,
            IAuthService authService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _roleManager = roleManager;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new User { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));

            await _userManager.AddToRoleAsync(user, "User");
            return Ok(new { Message = "Registro exitoso" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Credenciales inválidas");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _authService.GenerateJwtToken(user, roles);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours)
            });

            return Ok(new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours),
                User = _mapper.Map<UserProfileDto>(user)
            });
        }
    }
}
