namespace TaskBoard.api.Models.Dtos.BoardDtos
{
    public class BoardPermissionsDto
    {
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanInvite { get; set; }
    }
}
