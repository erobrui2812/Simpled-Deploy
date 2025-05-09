using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simpled.Dtos.BoardInvitations;
using Simpled.Models;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar invitaciones a tableros.
    /// </summary>
    public interface IBoardInvitationRepository
    {
        /// <summary>
        /// Obtiene todas las invitaciones asociadas a una dirección de correo.
        /// </summary>
        /// <param name="email">Correo electrónico del invitado.</param>
        /// <returns>Secuencia de <see cref="BoardInvitationReadDto"/>.</returns>
        Task<IEnumerable<BoardInvitationReadDto>> GetAllByEmailAsync(string email);

        /// <summary>
        /// Obtiene una invitación por su token único.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <returns>
        /// <see cref="BoardInvitationReadDto"/> correspondiente,
        /// o <c>null</c> si no existe.
        /// </returns>
        Task<BoardInvitationReadDto?> GetByTokenAsync(string token);

        /// <summary>
        /// Crea una nueva invitación a un tablero.
        /// </summary>
        /// <param name="dto">DTO con los datos de la invitación.</param>
        /// <returns>Entidad <see cref="BoardInvitation"/> creada.</returns>
        Task<BoardInvitation> CreateAsync(BoardInvitationCreateDto dto);

        /// <summary>
        /// Acepta una invitación existente.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <param name="userId">Identificador del usuario que acepta.</param>
        /// <returns><c>true</c> si la aceptación fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> AcceptAsync(string token, Guid userId);

        /// <summary>
        /// Rechaza una invitación existente.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <returns><c>true</c> si el rechazo fue exitoso; de lo contrario, <c>false</c>.</returns>
        Task<bool> RejectAsync(string token);
    }
}
