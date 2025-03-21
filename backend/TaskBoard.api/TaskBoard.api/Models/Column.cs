using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Models
{
    public class Column
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }

        public Board Board { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
