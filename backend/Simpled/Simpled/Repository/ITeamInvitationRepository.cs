using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simpled.Dtos.TeamInvitations;
using Simpled.Models;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar las invitaciones a equipos.
    /// </summary>
    public interface ITeamInvitationRepository
    {
        /// <summary>
        /// Obtiene todas las invitaciones asociadas a un correo electrónico.
        /// </summary>
        /// <param name="email">Correo del invitado.</param>
        /// <returns>Secuencia de DTOs de invitaciones.</returns>
        Task<IEnumerable<TeamInvitationReadDto>> GetAllByEmailAsync(string email);

        /// <summary>
        /// Obtiene una invitación por su token.
        /// </summary>
        /// <param name="token">Token único de la invitación.</param>
        /// <returns>DTO de la invitación, o <c>null</c> si no existe.</returns>
        Task<TeamInvitationReadDto?> GetByTokenAsync(string token);

        /// <summary>
        /// Crea una nueva invitación a un equipo.
        /// </summary>
        /// <param name="dto">DTO con los datos de la invitación.</param>
        /// <returns>Entidad de invitación creada.</returns>
        Task<TeamInvitation> CreateAsync(TeamInvitationCreateDto dto);

        /// <summary>
        /// Acepta una invitación existente.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <param name="userId">Identificador del usuario que acepta.</param>
        /// <returns><c>true</c> si la aceptación fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> AcceptAsync(string token, Guid userId);

        /// <summary>
        /// Rechaza una invitación.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <returns><c>true</c> si el rechazo fue exitoso; en caso contrario, <c>false</c>.</returns>
        Task<bool> RejectAsync(string token);
    }
}
