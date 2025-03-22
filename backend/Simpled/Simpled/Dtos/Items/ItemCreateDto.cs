namespace Simpled.Dtos.Items
{

    public class ItemCreateDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid ColumnId { get; set; }
    }
}
