using System.ComponentModel.DataAnnotations;

namespace Simpled.Models
{
    /// <summary>
    /// Representa un tablero que contiene columnas, ítems y miembros.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Identificador único del tablero.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del tablero.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Identificador del usuario propietario del tablero.
        /// </summary>
        [Required]
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Indica si el tablero es público.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Usuario propietario del tablero.
        /// </summary>
        public User? Owner { get; set; }

        /// <summary>
        /// Columnas asociadas al tablero.
        /// </summary>
        public List<BoardColumn> Columns { get; set; } = new();

        /// <summary>
        /// Miembros asociados al tablero.
        /// </summary>
        public List<BoardMember> BoardMembers { get; set; } = new();
    }
}
