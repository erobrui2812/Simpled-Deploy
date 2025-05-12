
using Simpled.Dtos.Columns;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar columnas de tableros.
    /// </summary>
    public interface IColumnRepository
    {
        /// <summary>
        /// Obtiene todas las columnas existentes.
        /// </summary>
        /// <returns>Secuencia de <see cref="BoardColumnReadDto"/>.</returns>
        Task<IEnumerable<BoardColumnReadDto>> GetAllAsync();

        /// <summary>
        /// Obtiene una columna por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la columna.</param>
        /// <returns>DTO <see cref="BoardColumnReadDto"/>, o <c>null</c> si no existe.</returns>
        Task<BoardColumnReadDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Crea una nueva columna en un tablero.
        /// </summary>
        /// <param name="dto">DTO con los datos de creación.</param>
        /// <returns>DTO <see cref="BoardColumnReadDto"/> de la columna creada.</returns>
        Task<BoardColumnReadDto> CreateAsync(BoardColumnCreateDto dto);

        /// <summary>
        /// Actualiza los datos de una columna existente.
        /// </summary>
        /// <param name="dto">DTO con los datos de actualización.</param>
        /// <returns><c>true</c> si la actualización fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> UpdateAsync(BoardColumnUpdateDto dto);

        /// <summary>
        /// Elimina una columna por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la columna.</param>
        /// <returns><c>true</c> si la operación fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Elimina una columna y, opcionalmente, sus ítems o los mueve a otra columna.
        /// </summary>
        /// <param name="columnId">Identificador de la columna.</param>
        /// <param name="cascadeItems">
        /// <c>true</c> para eliminar también los ítems; 
        /// <c>false</c> para moverlos.
        /// </param>
        /// <param name="targetColumnId">
        /// Identificador de la columna destino si <paramref name="cascadeItems"/> es <c>false</c>.
        /// </param>
        /// <returns><c>true</c> si la operación fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> DeleteAsync(Guid columnId, bool cascadeItems = false, Guid? targetColumnId = null);

        /// <summary>
        /// Obtiene el identificador del tablero al que pertenece una columna.
        /// </summary>
        /// <param name="columnId">Identificador de la columna.</param>
        /// <returns>Identificador del tablero asociado.</returns>
        Task<Guid> GetBoardIdByColumnId(Guid columnId);
    }
}
