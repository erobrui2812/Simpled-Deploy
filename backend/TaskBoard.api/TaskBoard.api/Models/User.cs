using Microsoft.AspNetCore.Identity;

namespace TaskBoard.api.Models
{
    public class User : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Board> OwnedBoards { get; set; } = new List<Board>();
        public ICollection<BoardMember> MemberBoards { get; set; } = new List<BoardMember>();
    }
}


