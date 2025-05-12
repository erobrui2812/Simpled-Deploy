namespace Simpled.Dtos.Dependencies
{
    public class DependencyReadDto
    {
        /// <summary>
        /// Identificador único de la dependencia.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador de la tarea origen.
        /// </summary>
        public Guid FromTaskId { get; set; }

        /// <summary>
        /// Identificador de la tarea destino.
        /// </summary>
        public Guid ToTaskId { get; set; }

        /// <summary>
        /// Tipo de dependencia.
        /// </summary>
        public string Type { get; set; } = string.Empty;
    }
}