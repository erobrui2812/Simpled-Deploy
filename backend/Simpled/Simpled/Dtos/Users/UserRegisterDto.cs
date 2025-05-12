using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Users
{
    /// <summary>
    /// DTO para el registro de un nuevo usuario.
    /// </summary>
    public class UserRegisterDto
    {
        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;
    }
}
