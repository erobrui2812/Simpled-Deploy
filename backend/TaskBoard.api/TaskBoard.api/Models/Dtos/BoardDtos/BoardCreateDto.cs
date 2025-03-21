using System.ComponentModel.DataAnnotations;

namespace TaskBoard.api.Models.Dtos.BoardDtos
{
    public class BoardCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public bool IsPublic { get; set; } = false;
    }
}

