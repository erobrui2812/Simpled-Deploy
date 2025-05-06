using System;

namespace Simpled.Models
{

    public class Dependency
    {
        public Guid Id { get; set; }

        public Guid FromTaskId { get; set; }
 
        public Item FromTask { get; set; } = null!;

        public Guid ToTaskId { get; set; }
   
        public Item ToTask { get; set; } = null!;

        public string Type { get; set; } = string.Empty;

        public Guid BoardId { get; set; }
    }
}
