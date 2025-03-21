using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Models
{
    public class BoardMember
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } // "admin|editor|viewer"

        public Board Board { get; set; }
        public User User { get; set; }
    }
}