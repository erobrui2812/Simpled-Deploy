using System;

namespace Simpled.Dtos.Subtasks
{
    public class SubtaskDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string Title { get; set; } = default!;
        public bool IsCompleted { get; set; }
    }
}
