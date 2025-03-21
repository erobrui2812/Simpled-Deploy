using TaskBoard.api.Models.Dtos.BoardDtos;

namespace TaskBoard.api.Models.Dtos.Board
{
    public class BoardDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public List<ColumnDto> Columns { get; set; }
        public BoardPermissionsDto Permissions { get; set; }
    }
}