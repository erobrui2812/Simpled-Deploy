namespace Simpled.Dtos.Dependencies
{
   
    public class DependencyCreateDto
    {
        public Guid FromTaskId { get; set; }

        public Guid ToTaskId { get; set; }

        public string Type { get; set; } = "finish-to-start";

        public Guid BoardId { get; set; }
    }
}