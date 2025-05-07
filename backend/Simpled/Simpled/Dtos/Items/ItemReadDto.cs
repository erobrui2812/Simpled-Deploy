using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Simpled.Dtos.Subtasks;

namespace Simpled.Dtos.Items
{
    public class ItemReadDto
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }

        [Required]
        public Guid ColumnId { get; set; }

        [Required]
        [RegularExpression("pending|in-progress|completed|delayed")]
        public string Status { get; set; } = default!;

        public Guid? AssigneeId { get; set; }

        public List<SubtaskDto> Subtasks { get; set; } = new();

        /// <summary>
        /// Porcentaje de subtareas completadas (0–100)
        /// </summary>
        public double Progress
            => Subtasks.Count == 0
                ? 0
                : Math.Round(Subtasks.Count(s => s.IsCompleted) * 100.0 / Subtasks.Count, 2);
    }
}
