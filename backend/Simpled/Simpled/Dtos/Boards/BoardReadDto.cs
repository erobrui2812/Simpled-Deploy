namespace Simpled.Dtos.Boards
{
    public class BoardReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsPublic { get; set; }
        public string? UserRole { get; set; } 
        public bool? IsFavorite { get; set; }
    }

}
