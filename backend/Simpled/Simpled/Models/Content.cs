using Simpled.Models.TrelloNotionClone.Models;

namespace Simpled.Models
{
    public class Content
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string Type { get; set; } = default!;  // "text|image|checkbox"
        public string Value { get; set; } = default!;

        public Item? Item { get; set; }
    }
}
