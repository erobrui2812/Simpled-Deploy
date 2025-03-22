namespace Simpled.Dtos.Columns
{
    public class BoardColumnUpdateDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string Title { get; set; } = default!;
        public int Order { get; set; }
    }
}
