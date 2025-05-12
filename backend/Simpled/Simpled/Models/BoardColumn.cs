using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
  /// <summary>
    /// Representa una columna dentro de un tablero.
    /// </summary>
    public class BoardColumn
    {
        /// <summary>
        /// Identificador único de la columna.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del tablero al que pertenece la columna.
        /// </summary>
        [Required]
        public Guid BoardId { get; set; }

        /// <summary>
        /// Título de la columna.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Orden de visualización de la columna.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Tablero al que pertenece esta columna.
        /// </summary>
        public Board? Board { get; set; }

        /// <summary>
        /// Ítems contenidos en esta columna.
        /// </summary>
        public List<Item> Items { get; set; } = new();
    }
}
