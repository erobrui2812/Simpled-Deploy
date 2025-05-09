using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Teams
{
    /// <summary>
    /// DTO para la creación de un equipo.
    /// </summary>
    public class TeamCreateDto
    {
        /// <summary>
        /// Nombre del equipo.
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
    }
}
