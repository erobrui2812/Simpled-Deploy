using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Auth
{
    /// <summary>
    /// DTO para petición de inicio de sesión con credenciales.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        /// <summary>
        /// Contraseña en texto plano.
        /// </summary>
        [Required]
        public string Password { get; set; } = default!;
    }
}
