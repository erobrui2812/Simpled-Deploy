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
    /// Implementa IActivityLogRepository.
    /// </summary>
    public class ActivityLogService : IActivityLogRepository
    {
        private readonly SimpledDbContext _context;

        public ActivityLogService(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene los logs de actividad asociados a un ítem.
        /// </summary>
        /// <param name="itemId">ID del ítem.</param>
        /// <returns>Lista de logs de actividad.</returns>
        /// <inheritdoc />  
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
                    UserName = a.User.Email,
                    UserAvatarUrl = null,
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
        /// Añade un nuevo log de actividad.
        /// </summary>
        /// <param name="log">Entidad ActivityLog a añadir.</param>
        /// <inheritdoc />  
        public async Task AddAsync(ActivityLog log)
        {
            if (log == null)
                throw new ApiException("El log de actividad no puede ser nulo.", 400);
            if (log.ItemId == Guid.Empty || log.UserId == Guid.Empty)
                throw new ApiException("El ID del ítem y del usuario son obligatorios.", 400);
            if (string.IsNullOrWhiteSpace(log.Action))
                throw new ApiException("La acción es obligatoria.", 400);
            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Parsea el tipo de actividad a partir de la acción.
        /// </summary>
        /// <param name="action">Acción registrada.</param>
        /// <returns>Tipo de actividad.</returns>
        private static ActivityType ParseActivityType(string action)
        {
            return Enum.TryParse<ActivityType>(action, out var result) ? result : ActivityType.Updated;
        }
    }
}
