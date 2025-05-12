using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.ActivityLogs;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exception;

namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión de logs de actividad.
    /// Implementa <see cref="IActivityLogRepository"/>.
    /// </summary>
    public class ActivityLogService : IActivityLogRepository
    {
        private readonly SimpledDbContext _context;

        /// <summary>
        /// Constructor que inyecta el contexto de datos.
        /// </summary>
        public ActivityLogService(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los registros de actividad asociados a un ítem, ordenados por fecha descendente.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        public async Task<IEnumerable<ActivityLogReadDto>> GetByItemIdAsync(Guid itemId)
        {
            return await _context.ActivityLogs
                .Include(a => a.User)
                .Where(a => a.ItemId == itemId)
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new ActivityLogReadDto
                {
                    Id = a.Id,
                    ItemId = a.ItemId,
                    UserId = a.UserId,
                    UserName = a.User.Name,
                    UserAvatarUrl = a.User.ImageUrl,
                    Type = ParseActivityType(a.Action),
                    Field = a.Field,
                    OldValue = a.OldValue,
                    NewValue = a.NewValue,
                    Details = a.Details,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();
        }

        /// <summary>
        /// Añade un nuevo registro de actividad.
        /// </summary>
        /// <param name="log">Entidad <see cref="ActivityLog"/> a registrar.</param>
        public async Task AddAsync(ActivityLog log)
        {
            if (log == null)
                throw new ApiException("El log de actividad no puede ser nulo.", 400);
            if (log.ItemId == Guid.Empty || log.UserId == Guid.Empty)
                throw new ApiException("El ID del ítem y del usuario son obligatorios.", 400);
            if (string.IsNullOrWhiteSpace(log.Action))
                throw new ApiException("El tipo de acción es obligatorio.", 400);

            // Asegurar timestamp UTC actual
            log.Timestamp = DateTime.UtcNow;

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Parsea el tipo de actividad a partir de la cadena de acción.
        /// </summary>
        /// <param name="action">Acción registrada.</param>
        private static ActivityType ParseActivityType(string action)
        {
            return Enum.TryParse<ActivityType>(action, ignoreCase: true, out var result)
                ? result
                : ActivityType.Updated;
        }
    }
}
