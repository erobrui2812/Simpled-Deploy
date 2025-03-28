namespace Simpled.Dtos.BoardMembers
{
    public class BoardMemberCreateDto
    {
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = default!;
    }
}
