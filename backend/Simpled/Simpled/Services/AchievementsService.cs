using Newtonsoft.Json;
using Simpled.Data;
using Simpled.Models;


namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión y procesamiento de logros
    /// </summary>
    public class AchievementsService
    {
        private readonly List<AchievementDefinition> _achievements;
        private readonly SimpledDbContext _context;

        public AchievementsService(SimpledDbContext context)
        {
            _context = context;

            var json = File.ReadAllText("Resources/achievements.json");
            _achievements = JsonConvert.DeserializeObject<List<AchievementDefinition>>(json)!;
        }

        /// <summary>
        /// Procesa una acción del usuario y desbloquea logros si corresponde.
        /// </summary>
        /// <param name="user">Usuario que realiza la acción.</param>
        /// <param name="action">Acción realizada.</param>
        /// <param name="newValue">Nuevo valor asociado a la acción.</param>
        /// <returns>Lista de mensajes de logros desbloqueados.</returns>
        public async Task<List<string>> ProcessActionAsync(User user, string action, int newValue)
        {
            var newAchievements = new List<string>();

            var achievementsPerAction = _achievements
                .Where(l => l.Action == action && l.Value <= newValue)
                .ToList();

            var unlockedAchievements = _context.UserAchievements
                .Where(x => x.UserId == user.Id && x.Action == action)
                .Select(x => x.Value)
                .ToHashSet();

            foreach (var achievement in achievementsPerAction)
            {
                if (!unlockedAchievements.Contains(achievement.Value))
                {
                    var nuevoLogro = new UserAchievement
                    {
                        UserId = user.Id,
                        Action = achievement.Action,
                        Value = achievement.Value,
                        Date = DateTime.UtcNow
                    };

                    _context.UserAchievements.Add(nuevoLogro);
                    newAchievements.Add(achievement.Message);
                }
            }

            if (newAchievements.Any())
            {
                await _context.SaveChangesAsync();
            }

            return newAchievements;
        }

        /// <summary>
        /// Obtiene todos los logros definidos en el sistema.
        /// </summary>
        /// <returns>Lista de logros.</returns>
        public List<object> GetAllAchievements()
        {
            var achievements = _achievements
                .Select(a => new
                {
                    Key = $"{a.Action}{a.Value}",
                    Title = a.Title,
                    Description = a.Description,
                })
                .ToList();

            return achievements.Cast<object>().ToList();
        }

        /// <summary>
        /// Definición interna de un logro.
        /// </summary>
        private class AchievementDefinition
        {
            public int Value { get; set; }
            public string Action { get; set; } = "";
            public string Message { get; set; } = "";
            public string Description { get; set; } = "";
            public string Title { get; set; } = "";
        }
    }
}
