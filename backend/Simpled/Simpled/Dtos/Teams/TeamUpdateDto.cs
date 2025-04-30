
using System;
using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Teams
{
    public class TeamUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
    }
}
