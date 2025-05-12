using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{

    /// <summary>
    /// Representa una relación de dependencia entre dos tareas.
    /// </summary>
    public class Dependency
    {
        /// <summary>
        /// Identificador único de la dependencia.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador de la tarea dependiente.
        /// </summary>
        [Required]
        public Guid FromTaskId { get; set; }

        /// <summary>
        /// Tarea que depende de otra.
        /// </summary>
        public Item FromTask { get; set; } = null!;

        /// <summary>
        /// Identificador de la tarea prerequisito.
        /// </summary>
        [Required]
        public Guid ToTaskId { get; set; }

        /// <summary>
        /// Tarea que actúa como prerequisito.
        /// </summary>
        public Item ToTask { get; set; } = null!;

        /// <summary>
        /// Tipo de dependencia.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del tablero que contiene las tareas.
        /// </summary>
        public Guid BoardId { get; set; }
    }

}
