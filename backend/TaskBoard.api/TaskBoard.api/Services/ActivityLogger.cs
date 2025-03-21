using TaskBoard.api.Models;
using TaskBoard.api.Data;
using System.Text.Json;

namespace TaskBoard.api.Services
{
    public class ActivityLogger
    {
        private readonly AppDbContext _context;

        public ActivityLogger(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActivity(
            Guid boardId,
            Guid userId,
            string actionType,
            string targetType,
            object data)
        {
            var log = new Activity
            {
                BoardId = boardId,
                UserId = userId,
                Action = actionType,
                EntityType = targetType,
                Details = JsonSerializer.Serialize(data),
                Timestamp = DateTime.UtcNow
            };

            await _context.Activities.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogItemMoved(Guid boardId, Guid userId, Item item)
        {
            var log = new Activity
            {
                BoardId = boardId,
                UserId = userId,
                Action = "ItemMoved",
                EntityType = "Item",
                Details = JsonSerializer.Serialize(new
                {
                    FromColumn = item.ColumnId,
                    ToColumn = item.ColumnId,
                    ItemId = item.Id
                }),
                Timestamp = DateTime.UtcNow
            };

            await _context.Activities.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}



