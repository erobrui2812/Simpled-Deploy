namespace Simpled.Dtos.Boards
{

    public class BoardReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid OwnerId { get; set; }
        public bool IsPublic { get; set; }
    }
}
