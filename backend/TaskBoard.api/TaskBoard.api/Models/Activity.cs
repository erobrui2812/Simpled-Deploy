using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Models
{
    public class Activity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
