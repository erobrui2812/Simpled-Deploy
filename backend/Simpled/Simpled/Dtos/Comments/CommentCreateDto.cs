using System;
using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Comments
{
    public class CommentCreateDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;
    }
}
