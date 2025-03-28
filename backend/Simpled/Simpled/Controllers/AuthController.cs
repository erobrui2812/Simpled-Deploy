using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Auth;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authService;

        public AuthController(IAuthRepository authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);
            return token == null ? Unauthorized("Credenciales inválidas") : Ok(new { token });
        }
    }
}
