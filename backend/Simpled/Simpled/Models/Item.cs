namespace Simpled.Models
{
    // Models/Item.cs
    namespace TrelloNotionClone.Models
    {
        public class Item
        {
            public Guid Id { get; set; }
            public Guid ColumnId { get; set; }
            public string Title { get; set; } = default!;
            public string? Description { get; set; }
            public DateTime? DueDate { get; set; }

            public BoardColumn? Column { get; set; }
            public List<Content> Contents { get; set; } = new();
        }
    }

}
