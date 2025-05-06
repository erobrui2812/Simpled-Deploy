using System;
using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class Subtask
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        public bool IsCompleted { get; set; } = false;

        public Item Item { get; set; } = default!;
    }
}
