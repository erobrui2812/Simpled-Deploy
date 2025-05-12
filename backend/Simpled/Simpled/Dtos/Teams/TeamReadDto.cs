using Simpled.Dtos.Teams.TeamMembers;

namespace Simpled.Dtos.Teams
{
    /// <summary>
    /// DTO para la lectura de un equipo.
    /// </summary>
    public class TeamReadDto
    {
        /// <summary>
        /// Identificador único del equipo.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del equipo.
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Identificador del propietario del equipo.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Nombre del propietario del equipo.
        /// </summary>
        public string OwnerName { get; set; } = default!;

        /// <summary>
        /// Rol del usuario en el equipo.
        /// </summary>
        public string Role { get; set; } = default!;

        /// <summary>
        /// Miembros del equipo.
        /// </summary>
        public IEnumerable<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
    }
}
