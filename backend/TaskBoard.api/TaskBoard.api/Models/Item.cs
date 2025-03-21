using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Models
{
    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ColumnId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public Column Column { get; set; }

        public int Order { get; set; }
        public List<Content> Contents { get; set; } = new List<Content>();
    }
}
