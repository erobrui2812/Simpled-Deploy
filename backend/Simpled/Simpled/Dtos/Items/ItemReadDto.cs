namespace Simpled.Dtos.Items
{
    public class ItemReadDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? ColumnId { get; set; }
    }
}
