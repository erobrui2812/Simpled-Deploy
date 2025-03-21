namespace TaskBoard.api.Models.Dtos
{
    public class ItemMoveDto
    {
        public Guid BoardId { get; set; }
        public Guid ItemId { get; set; }
        public Guid FromColumnId { get; set; }
        public Guid ToColumnId { get; set; }
    }
}


