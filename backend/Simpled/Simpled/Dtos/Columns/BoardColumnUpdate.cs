using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Columns
{
    public class BoardColumnUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid BoardId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = default!;

        public int Order { get; set; }
    }
}
