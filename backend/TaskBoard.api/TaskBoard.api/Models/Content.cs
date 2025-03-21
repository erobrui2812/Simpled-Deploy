using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Models
{
    public class Content
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string Type { get; set; } // "text|image|checkbox"
        public string Value { get; set; }

        public Item Item { get; set; }
    }
}


