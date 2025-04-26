using Microsoft.AspNetCore.Authorization;
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
        /// Inicia sesión y devuelve un token JWT
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

        /// <summary>
        /// Inicia sesión con Google y devuelve un token JWT
        /// </summary>
        /// <param name="googleLoginDto">Token de Google</param>
        /// <returns>Token JWT o solicitud de verificación</returns>
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            var result = await _authService.LoginWithGoogleAsync(googleLoginDto);

            if (!result.Success)
                return Unauthorized(result.ErrorMessage);

            return Ok(new
            {
                token = result.Token,
                id = result.UserId,
                needsVerification = result.NeedsVerification
            });
        }

        /// <summary>
        /// Confirma el correo electrónico usando un código enviado.
        /// </summary>
        /// <param name="confirmDto">Datos de confirmación</param>
        /// <returns>Éxito o error</returns>
        [HttpPost("confirm-code")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmDto)
        {
            var result = await _authService.ConfirmEmailAsync(confirmDto);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(new
            {
                message = "Email confirmado exitosamente."
            });
        }


    }
}
