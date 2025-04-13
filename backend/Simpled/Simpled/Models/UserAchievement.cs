namespace Simpled.Models
{
    public class UserAchievement
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string Accion { get; set; } = null!;
        public int Valor { get; set; }
        public DateTime Fecha { get; set; }
    }

}
