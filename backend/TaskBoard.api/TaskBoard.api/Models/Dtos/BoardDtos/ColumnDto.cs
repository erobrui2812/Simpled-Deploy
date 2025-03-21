namespace TaskBoard.api.Models.Dtos.BoardDtos
{
    public class ColumnDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public List<ItemDto> Items { get; set; }
    }
}


