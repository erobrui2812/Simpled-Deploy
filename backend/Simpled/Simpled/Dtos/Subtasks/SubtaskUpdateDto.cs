using System;
using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Subtasks
{
    public class SubtaskUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        public bool IsCompleted { get; set; }
    }
}
