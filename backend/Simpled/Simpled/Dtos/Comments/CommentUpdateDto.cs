using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Comments
{
    public class CommentUpdateDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public Guid CommentId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;
    }
}
