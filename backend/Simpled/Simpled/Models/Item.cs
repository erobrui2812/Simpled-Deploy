using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class Item
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ColumnId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        [RegularExpression("pending|in-progress|completed|delayed")]
        public string Status { get; set; } = "pending";

        public BoardColumn? Column { get; set; }
        public List<Content> Contents { get; set; } = new();
    }
}
