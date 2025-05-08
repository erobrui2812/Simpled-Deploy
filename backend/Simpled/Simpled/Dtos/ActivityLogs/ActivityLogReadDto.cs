namespace Simpled.Dtos.ActivityLogs
{
    
    public class ActivityLogReadDto
    {
      
        public Guid Id { get; set; }

       
        public Guid ItemId { get; set; }

   
        public Guid UserId { get; set; }


        public string UserName { get; set; } = string.Empty;

       
        public string? UserAvatarUrl { get; set; }

     
        public ActivityType Type { get; set; }

        
        public string? Field { get; set; }

      
        public string? OldValue { get; set; }

        
        public string? NewValue { get; set; }

        public string Details { get; set; } = string.Empty;

       
        public DateTime Timestamp { get; set; }
    }


    public enum ActivityType
    {
        Created,
        Updated,
        StatusChanged,
        Assigned,
        DateChanged,
        Deleted,
        FileUploaded,
        SubtaskCreated,
        SubtaskUpdated,
        SubtaskDeleted,
        CommentAdded,
        CommentEdited,
        CommentDeleted,
        CommentResolved
    }
}
