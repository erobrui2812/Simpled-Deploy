using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Models
{
    public class Board
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsPublic { get; set; }

        public User Owner { get; set; }
        public List<Column> Columns { get; set; } = new List<Column>();
        public List<BoardMember> Members { get; set; } = new List<BoardMember>();
    }
}