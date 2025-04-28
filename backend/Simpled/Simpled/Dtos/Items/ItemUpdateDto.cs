
using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Items
{
    public class ItemUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public Guid ColumnId { get; set; }

        [Required]
        [RegularExpression("pending|in-progress|completed|delayed",
            ErrorMessage = "El estado debe ser pending, in-progress, completed o delayed.")]
        public string Status { get; set; } = "pending";
    }
}
