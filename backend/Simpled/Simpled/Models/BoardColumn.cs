using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class BoardColumn
    {
        public Guid Id { get; set; }

        [Required]
        public Guid BoardId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = default!;

        public int Order { get; set; }

        public Board? Board { get; set; }
        public List<Item> Items { get; set; } = new();
    }
}
