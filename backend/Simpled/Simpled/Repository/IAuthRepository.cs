using System.Threading.Tasks;
using Simpled.Dtos.Auth;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones de autenticación de usuarios.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Intenta iniciar sesión con credenciales propias.
        /// </summary>
        /// <param name="loginDto">DTO con email y contraseña.</param>
        /// <returns>
        /// <see cref="LoginResultDto"/> con token y userId si la autenticación es
        /// correcta; <c>null</c> en caso contrario.
        /// </returns>
        Task<LoginResultDto?> LoginAsync(LoginRequestDto loginDto);

        /// <summary>
        /// Intenta iniciar sesión mediante proveedor externo.
        /// </summary>
        /// <param name="dto">DTO con datos del proveedor externo.</param>
        /// <returns>
        /// <see cref="LoginResultDto"/> con token y userId si la autenticación es
        /// correcta; <c>null</c> en caso contrario.
        /// </returns>
        Task<LoginResultDto?> ExternalLoginAsync(ExternalLoginDto dto);
    }
}
