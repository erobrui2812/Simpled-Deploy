using Simpled.Dtos.ActivityLogs;

namespace Simpled.Repository
{
    public interface IActivityLogRepository
    {
        Task<IEnumerable<ActivityLogReadDto>> GetByItemIdAsync(Guid itemId);
    }
}
