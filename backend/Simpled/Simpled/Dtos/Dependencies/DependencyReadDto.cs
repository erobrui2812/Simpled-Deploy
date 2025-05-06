namespace Simpled.Dtos.Dependencies
{
    public class DependencyReadDto
    {
        public Guid Id { get; set; }

        public Guid FromTaskId { get; set; }

        public Guid ToTaskId { get; set; }

        public string Type { get; set; } = string.Empty;
    }
}