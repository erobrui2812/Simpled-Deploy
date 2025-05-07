using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Items
{
    public class ItemCreateDto
    {
        [Required, MaxLength(100)]
        public string Title { get; set; } = default!;
        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        public Guid ColumnId { get; set; }
        [Required]
        [RegularExpression("pending|in-progress|completed|delayed")]
        public string Status { get; set; } = "pending";

        public Guid? AssigneeId { get; set; }
    }
}
