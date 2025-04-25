namespace Simpled.Dtos.Teams
{
    public class TeamDto
    {
        //TODO: Arreglar la key para poner el guid del grupo
        public string Key { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
