using Newtonsoft.Json;
using Simpled.Data;
using Simpled.Models;

public class AchievementsService
{
    private readonly List<AchievementDefinition> _logros;
    private readonly SimpledDbContext _context;

    public AchievementsService(SimpledDbContext context)
    {
        _context = context;

        var json = File.ReadAllText("Resources/achievements.json");
        _logros = JsonConvert.DeserializeObject<List<AchievementDefinition>>(json)!;
    }

    public async Task<List<string>> ProcesarAccionAsync(User user, string accion, int nuevoValor)
    {
        var nuevosLogros = new List<string>();

        var logrosPorAccion = _logros
            .Where(l => l.Action == accion && l.Value == nuevoValor)
            .ToList();

        foreach (var logro in logrosPorAccion)
        {
            var yaDesbloqueado = _context.UserAchievements
                .Any(x => x.UserId == user.Id && x.Accion == logro.Action && x.Valor == logro.Value);

            if (!yaDesbloqueado)
            {
                var nuevoLogro = new UserAchievement
                {
                    UserId = user.Id,
                    Accion = logro.Action,
                    Valor = logro.Value,
                    Fecha = DateTime.UtcNow
                };

                _context.UserAchievements.Add(nuevoLogro);
                nuevosLogros.Add(logro.Message);
            }
        }

        if (nuevosLogros.Any())
        {
            await _context.SaveChangesAsync();
        }

        return nuevosLogros;
    }

    private class AchievementDefinition
    {
        public int Value { get; set; }
        public string Action { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
