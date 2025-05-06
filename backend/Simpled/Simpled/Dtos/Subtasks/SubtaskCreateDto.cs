using System;
using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Subtasks
{
    public class SubtaskCreateDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;
    }
}
