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
    /// Implementa IActivityLogRepository.
    /// </summary>
    public class ActivityLogService : IActivityLogRepository
    {
        private readonly SimpledDbContext _context;

        public ActivityLogService(SimpledDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ActivityLogReadDto>> GetByItemIdAsync(Guid itemId)
        {
            
            var logs = await _context.ActivityLogs
                .Include(a => a.User)
                .Where(a => a.ItemId == itemId)
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new ActivityLogReadDto
                {
                    Id = a.Id,
                    ItemId = a.ItemId,
                    UserId = a.UserId,
                    UserName = a.User.Email,
                    UserAvatarUrl = a.User.ImageUrl,
                    Type = ParseActivityType(a.Action),
                    Field = a.Field,
                    OldValue = a.OldValue,
                    NewValue = a.NewValue,
                    Details = a.Details,
                    Timestamp = new DateTimeOffset(a.Timestamp, TimeSpan.Zero)
                })
                .ToListAsync();

            var userIdStrings = logs
                .Where(l => l.Type == ActivityType.Assigned)
                .SelectMany(l => new[] { l.OldValue, l.NewValue })
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .ToList();

            var guidSet = userIdStrings
                .Select(s => Guid.TryParse(s, out var g) ? g : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g!.Value)
                .ToHashSet();

            if (guidSet.Any())
            {
                var users = await _context.Users
                    .Where(u => guidSet.Contains(u.Id))
                    .Select(u => new { u.Id, u.Name })
                    .ToListAsync();

                var nameMap = users.ToDictionary(u => u.Id.ToString(), u => u.Name);


                foreach (var log in logs.Where(l => l.Type == ActivityType.Assigned))
                {
                    if (log.OldValue != null && nameMap.TryGetValue(log.OldValue, out var oldName))
                        log.OldValueName = oldName;
                    if (log.NewValue != null && nameMap.TryGetValue(log.NewValue, out var newName))
                        log.NewValueName = newName;
                }
            }

            return logs;
        }

        /// <inheritdoc/>
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
        private static ActivityType ParseActivityType(string action)
            => Enum.TryParse<ActivityType>(action, out var result)
               ? result
               : ActivityType.Updated;
    }
}
