using Simpled.Models.Simpled.Models;
using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    public class Board
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public Guid OwnerId { get; set; }

        public bool IsPublic { get; set; }

        public User? Owner { get; set; }
        public List<BoardColumn> Columns { get; set; } = new();
        public List<BoardMember> BoardMembers { get; set; } = new();
    }
}
