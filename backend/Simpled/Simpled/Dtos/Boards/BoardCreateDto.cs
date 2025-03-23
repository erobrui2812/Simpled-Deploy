namespace Simpled.Dtos.Boards
{

    public class BoardCreateDto
    {
        public string Name { get; set; } = default!;
        public Guid OwnerId { get; set; }
        public bool IsPublic { get; set; }
    }
}
