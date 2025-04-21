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

        /// <summary>
        /// Inicia sesión y devuelve un token JWT válido si las credenciales son correctas.
        /// </summary>
        /// <param name="loginDto">Datos de acceso del usuario</param>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new
            {
                token = result.Token,
                id = result.UserId
            });
        }
    }
}
