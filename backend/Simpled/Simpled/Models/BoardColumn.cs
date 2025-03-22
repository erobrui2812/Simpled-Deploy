namespace Simpled.Models
{

    namespace TrelloNotionClone.Models
    {
        public class BoardColumn
        {
            public Guid Id { get; set; }
            public Guid BoardId { get; set; }
            public string Title { get; set; } = default!;
            public int Order { get; set; }

            public Board? Board { get; set; }
            public List<Item> Items { get; set; } = new();
        }
    }

}
