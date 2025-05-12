namespace Simpled.Dtos.Achievements
{
    /// <summary>
    /// DTO que representa un logro desbloqueado por un usuario.
    /// </summary>
    public class UnlockedAchievementDto
    {
        /// <summary>
        /// Clave única que identifica el logro.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Fecha en que se desbloqueó el logro (UTC).
        /// </summary>
        public DateTime unlockedDate { get; set; }
    }
}