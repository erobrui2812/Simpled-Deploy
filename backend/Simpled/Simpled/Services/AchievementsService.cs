using Newtonsoft.Json;
using Simpled.Models;

public class AchievementsService
{
    private readonly List<AchievementDefinition> _logros;

    public AchievementsService()
    {
        var json = File.ReadAllText("Resources/achievements.json");
        _logros = JsonConvert.DeserializeObject<List<AchievementDefinition>>(json)!;
    }

    public List<string> ProcesarAccion(User user, string accion, int nuevoValor)
    {
        var nuevosLogros = new List<string>();

        var logrosPorAccion = _logros
            .Where(l => l.Action == accion && l.Value == nuevoValor)
            .ToList();

        foreach (var logro in logrosPorAccion)
        {
            var yaDesbloqueado = user.Logros.Any(x => x.Accion == logro.Action && x.Valor == logro.Value);
            if (!yaDesbloqueado)
            {
                user.Logros.Add(new UserAchievement
                {
                    UserId = user.Id,
                    Accion = logro.Action,
                    Valor = logro.Value,
                    Fecha = DateTime.UtcNow
                });

                nuevosLogros.Add(logro.Message);
            }
        }

        return nuevosLogros;
    }

    public class AchievementDefinition
    {
        public int Value { get; set; }
        public string Action { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
