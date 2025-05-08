namespace Simpled.Dtos.ActivityLogs
{
    public class ActivityLogReadDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatarUrl { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
