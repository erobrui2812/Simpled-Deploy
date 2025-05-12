
using Simpled.Dtos.Boards;
using Simpled.Models;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar tableros.
    /// </summary>
    public interface IBoardRepository
    {
        /// <summary>
        /// Obtiene todos los tableros, opcionalmente filtrando por usuario.
        /// </summary>
        /// <param name="userId">
        /// Identificador del usuario para mostrar sus tableros;
        /// <c>null</c> para todos los públicos y privados.
        /// </param>
        /// <returns>Secuencia de <see cref="BoardReadDto"/>.</returns>
        Task<IEnumerable<BoardReadDto>> GetAllAsync(Guid? userId = null);

        /// <summary>
        /// Obtiene un tablero por su identificador para un usuario.
        /// </summary>
        /// <param name="id">Identificador del tablero.</param>
        /// <param name="userId">Identificador del usuario solicitante.</param>
        /// <returns>DTO <see cref="BoardReadDto"/>, o <c>null</c> si no tiene acceso.</returns>
        Task<BoardReadDto?> GetByIdAsync(Guid id, Guid userId);

        /// <summary>
        /// Crea un nuevo tablero para un usuario.
        /// </summary>
        /// <param name="dto">DTO con los datos de creación.</param>
        /// <param name="userId">Identificador del propietario.</param>
        /// <returns>DTO <see cref="BoardReadDto"/> del tablero creado.</returns>
        Task<BoardReadDto> CreateAsync(BoardCreateDto dto, Guid userId);

        /// <summary>
        /// Actualiza los datos de un tablero.
        /// </summary>
        /// <param name="dto">DTO con los datos de actualización.</param>
        /// <returns><c>true</c> si la actualización fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> UpdateAsync(BoardUpdateDto dto);

        /// <summary>
        /// Elimina un tablero por su identificador.
        /// </summary>
        /// <param name="id">Identificador del tablero.</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Obtiene la entidad <see cref="Board"/> sin mapear a DTO.
        /// </summary>
        /// <param name="id">Identificador del tablero.</param>
        /// <returns>Entidad <see cref="Board"/>, o <c>null</c> si no existe.</returns>
        Task<Board?> GetBoardByIdAsync(Guid id);
    }
}
