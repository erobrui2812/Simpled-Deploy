using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simpled.Dtos.Teams.TeamMembers;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones de gestión de miembros dentro de un equipo.
    /// </summary>
    public interface ITeamMemberRepository
    {
        /// <summary>
        /// Obtiene los miembros de un equipo.
        /// </summary>
        /// <param name="teamId">Identificador del equipo.</param>
        /// <returns>Secuencia de DTOs de miembros.</returns>
        Task<IEnumerable<TeamMemberDto>> GetMembersAsync(Guid teamId);

        /// <summary>
        /// Añade un nuevo miembro a un equipo.
        /// </summary>
        /// <param name="dto">DTO con los datos del nuevo miembro.</param>
        /// <param name="ownerId">Identificador del propietario (para validaciones).</param>
        Task AddMemberAsync(TeamMemberCreateDto dto, Guid ownerId);

        /// <summary>
        /// Actualiza el rol de un miembro en un equipo.
        /// </summary>
        /// <param name="dto">DTO con los datos de actualización.</param>
        /// <param name="ownerId">Identificador del propietario (para validaciones).</param>
        /// <returns><c>true</c> si la actualización fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> UpdateMemberAsync(TeamMemberUpdateDto dto, Guid ownerId);

        /// <summary>
        /// Elimina un miembro de un equipo.
        /// </summary>
        /// <param name="teamId">Identificador del equipo.</param>
        /// <param name="userId">Identificador del usuario a eliminar.</param>
        /// <param name="ownerId">Identificador del propietario (para validaciones).</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> RemoveMemberAsync(Guid teamId, Guid userId, Guid ownerId);
    }
}
