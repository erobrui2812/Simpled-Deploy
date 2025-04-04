using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class Content
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [RegularExpression("text|image|checkbox")]
        public string Type { get; set; } = default!;

        [Required]
        [MaxLength(2048)]
        public string Value { get; set; } = default!;

        public Item? Item { get; set; }
    }
}
