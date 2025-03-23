namespace Simpled.Dtos.Users
{

    public class UserUpdateDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string? Password { get; set; }
    }
}
