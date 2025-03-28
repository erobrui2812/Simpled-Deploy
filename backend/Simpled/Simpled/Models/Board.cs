using Simpled.Models.TrelloNotionClone.Models;

namespace Simpled.Models
{
    public class Board
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid OwnerId { get; set; }
        public bool IsPublic { get; set; }


        public User? Owner { get; set; }
        public List<BoardColumn> Columns { get; set; } = new();

        public List<BoardMember> BoardMembers { get; set; } = new();
    }
}
