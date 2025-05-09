using System;

namespace Simpled.Dtos.Subtasks
{
    /// <summary>
    /// DTO para la lectura de una subtarea.
    /// </summary>
    public class SubtaskDto
    {
        /// <summary>
        /// Identificador único de la subtarea.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del ítem al que pertenece la subtarea.
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// Título de la subtarea.
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// Indica si la subtarea está completada.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
