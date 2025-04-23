namespace Simpled.Models
{
    public class UserAchievement
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string Action { get; set; } = null!;
        public int Value { get; set; }
        public DateTime Date { get; set; }
    }

}
