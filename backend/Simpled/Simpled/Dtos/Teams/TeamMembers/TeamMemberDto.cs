/// <summary>
/// DTO para la información de un miembro de equipo.
/// </summary>
namespace Simpled.Dtos.Teams.TeamMembers
{
    /// <summary>
    /// DTO para la información de un miembro de equipo.
    /// </summary>
    public class TeamMemberDto
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string UserName { get; set; } = default!;

        /// <summary>
        /// Rol del usuario en el equipo.
        /// </summary>
        public string Role { get; set; } = default!;
    }
}
