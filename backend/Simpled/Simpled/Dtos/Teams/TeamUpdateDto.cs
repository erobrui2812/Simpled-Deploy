using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Teams
{
    /// <summary>
    /// DTO para actualización de un equipo.
    /// </summary>
    public class TeamUpdateDto
    {
        /// <summary>
        /// Identificador único del equipo.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Nuevo nombre del equipo (máx. 100 caracteres).
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
    }
}