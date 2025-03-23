namespace Simpled.Dtos.Columns
{
    public class BoardColumnCreateDto
    {
        public Guid BoardId { get; set; }
        public string Title { get; set; } = default!;
        public int Order { get; set; }
    }
}

