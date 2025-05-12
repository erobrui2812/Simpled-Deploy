namespace Simpled.Dtos.Dependencies
{
    /// <summary>
    /// DTO para la creación de una dependencia entre tareas.
    /// </summary>
    public class DependencyCreateDto
    {
        /// <summary>
        /// Identificador de la tarea origen.
        /// </summary>
        public Guid FromTaskId { get; set; }

        /// <summary>
        /// Identificador de la tarea destino.
        /// </summary>
        public Guid ToTaskId { get; set; }

        /// <summary>
        /// Tipo de dependencia (por defecto "finish-to-start").
        /// </summary>
        public string Type { get; set; } = "finish-to-start";

        /// <summary>
        /// Identificador del tablero al que pertenece la dependencia.
        /// </summary>
        public Guid BoardId { get; set; }
    }
}