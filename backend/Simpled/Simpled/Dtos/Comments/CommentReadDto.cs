using System;

namespace Simpled.Dtos.Comments
{
    public class CommentReadDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public bool IsResolved { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? EditedAt { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string? UserAvatarUrl { get; set; } // opcional si tienes imagen
    }
}
