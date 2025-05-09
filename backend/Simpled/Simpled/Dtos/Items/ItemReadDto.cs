using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Simpled.Dtos.Subtasks;

namespace Simpled.Dtos.Items
{
    /// <summary>
    /// DTO para la lectura de un ítem.
    /// </summary>
    public class ItemReadDto
    {
        /// <summary>
        /// Identificador único del ítem.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título del ítem.
        /// </summary>
        [Required, MaxLength(100)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Descripción del ítem (opcional).
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Fecha de vencimiento del ítem (opcional).
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Fecha de inicio del ítem (opcional).
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Identificador de la columna a la que pertenece el ítem.
        /// </summary>
        [Required]
        public Guid ColumnId { get; set; }

        /// <summary>
        /// Estado del ítem.
        /// </summary>
        [Required]
        [RegularExpression("pending|in-progress|completed|delayed")]
        public string Status { get; set; } = default!;

        /// <summary>
        /// Identificador del usuario asignado (opcional).
        /// </summary>
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// Lista de subtareas asociadas al ítem.
        /// </summary>
        public List<SubtaskDto> Subtasks { get; set; } = new();

        /// <summary>
        /// Porcentaje de subtareas completadas (0–100)
        /// </summary>
        public double Progress
            => Subtasks.Count == 0
                ? 0
                : Math.Round(Subtasks.Count(s => s.IsCompleted) * 100.0 / Subtasks.Count, 2);
    }
}
