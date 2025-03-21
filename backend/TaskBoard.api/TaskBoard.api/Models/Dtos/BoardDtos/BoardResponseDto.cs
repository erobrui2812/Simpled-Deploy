namespace TaskBoard.api.Models.Dtos.BoardDtos
{
    public class BoardResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ColumnDto> Columns { get; set; }
    }
}


