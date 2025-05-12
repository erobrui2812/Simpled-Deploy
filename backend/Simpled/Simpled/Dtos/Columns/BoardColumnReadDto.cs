
namespace Simpled.Dtos.Columns
{
    public class BoardColumnReadDto
    {
        /// <summary>
        /// Identificador único de la columna.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del tablero al que pertenece la columna.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Título de la columna.
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// Orden de la columna en el tablero.
        /// </summary>
        public int Order { get; set; }
    }
}
