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

        public BoardColumn? Column { get; set; }
        public List<Content> Contents { get; set; } = new();
    }
}
