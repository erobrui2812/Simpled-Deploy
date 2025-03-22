namespace TaskBoard.api.Models.Dtos.ColumnDtos
{
    public class ColumnResponseDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
    }
}
