using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa una invitación enviada a un usuario para unirse a un tablero.
    /// </summary>
    public class BoardInvitation
    {
        /// <summary>
        /// Identificador único de la invitación.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del tablero invitado.
        /// </summary>
        [Required]
        public Guid BoardId { get; set; }

        /// <summary>
        /// Correo electrónico del invitado.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Token utilizado para aceptar la invitación.
        /// </summary>
        [Required]
        public string Token { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Rol asignado al aceptar la Invitación.
        /// </summary>
        [Required]
        public string Role { get; set; } = "viewer";

        /// <summary>
        /// Indica si la invitación ha sido aceptada.
        /// </summary>
        public bool Accepted { get; set; } = false;

        /// <summary>
        /// Fecha de creación de la invitación.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Tablero asociado a la invitación.
        /// </summary>
        public Board? Board { get; set; }
    }

}
