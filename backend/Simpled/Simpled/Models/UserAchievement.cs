namespace Simpled.Models
{
    /// <summary>
    /// Representa un logro conseguido por el usuario.
    /// </summary>
    public class UserAchievement
    {
        /// <summary>
        /// Identificador único del logro.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del usuario relacionado con el logro.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Usuario asociado al logro.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Acción o hito del logro.
        /// </summary>
        public string Action { get; set; } = null!;

        /// <summary>
        /// Valor numérico asociado al logro.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Fecha en que se registró el logro (UTC).
        /// </summary>
        public DateTime Date { get; set; }
    }

}
