
using System.ComponentModel.DataAnnotations;

namespace Simpled.Dtos.Teams
{
    public class TeamCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
    }
}
