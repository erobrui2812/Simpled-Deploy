using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Boards
{
    public class BoardCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public Guid OwnerId { get; set; }

        public bool IsPublic { get; set; }
    }
}
