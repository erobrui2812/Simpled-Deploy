namespace TaskBoard.api.Models.Dtos.AuthDtos
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}



