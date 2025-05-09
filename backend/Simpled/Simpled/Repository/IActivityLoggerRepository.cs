using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simpled.Dtos.ActivityLogs;
using Simpled.Models;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar el historial de actividad.
    /// </summary>
    public interface IActivityLogRepository
    {
        /// <summary>
        /// Obtiene todos los registros de actividad asociados a un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <returns>Secuencia de <see cref="ActivityLogReadDto"/>.</returns>
        Task<IEnumerable<ActivityLogReadDto>> GetByItemIdAsync(Guid itemId);

        /// <summary>
        /// Añade un nuevo registro de actividad.
        /// </summary>
        /// <param name="log">Entidad <see cref="ActivityLog"/> a registrar.</param>
        Task AddAsync(ActivityLog log);
    }
}
