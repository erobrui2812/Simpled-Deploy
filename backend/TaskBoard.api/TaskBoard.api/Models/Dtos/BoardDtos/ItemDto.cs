namespace TaskBoard.api.Models.Dtos.BoardDtos
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}



