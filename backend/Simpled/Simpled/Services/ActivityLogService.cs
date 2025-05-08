using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.ActivityLogs;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    /// <summary>  
    /// Implementación de IActivityLogRepository usando EF Core.  
    /// </summary>  
    public class ActivityLogService : IActivityLogRepository
    {
        private readonly SimpledDbContext _context;

        public ActivityLogService(SimpledDbContext context)
        {
            _context = context;
        }

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

        /// <inheritdoc />  
        public async Task AddAsync(ActivityLog log)
        {
            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        private static ActivityType ParseActivityType(string action)
        {
            return Enum.TryParse<ActivityType>(action, out var result) ? result : ActivityType.Updated;
        }
    }
}
