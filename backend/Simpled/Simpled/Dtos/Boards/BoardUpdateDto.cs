using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Boards
{
    public class BoardUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public Guid OwnerId { get; set; }

        public bool IsPublic { get; set; }
    }
}
