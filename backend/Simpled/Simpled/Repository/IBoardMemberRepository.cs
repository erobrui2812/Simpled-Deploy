
using Simpled.Dtos.BoardMembers;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar miembros de tableros.
    /// </summary>
    public interface IBoardMemberRepository
    {
        /// <summary>
        /// Obtiene todos los miembros de todos los tableros.
        /// </summary>
        /// <returns>Secuencia de <see cref="BoardMemberReadDto"/>.</returns>
        Task<IEnumerable<BoardMemberReadDto>> GetAllAsync();

        /// <summary>
        /// Obtiene los miembros de un tablero específico.
        /// </summary>
        /// <param name="boardId">Identificador del tablero.</param>
        /// <returns>Secuencia de <see cref="BoardMemberReadDto"/>.</returns>
        Task<IEnumerable<BoardMemberReadDto>> GetByBoardIdAsync(Guid boardId);

        /// <summary>
        /// Obtiene un miembro de tablero por sus identificadores.
        /// </summary>
        /// <param name="boardId">Identificador del tablero.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>DTO <see cref="BoardMemberReadDto"/>, o <c>null</c> si no existe.</returns>
        Task<BoardMemberReadDto?> GetByIdsAsync(Guid boardId, Guid userId);

        /// <summary>
        /// Añade un nuevo miembro a un tablero.
        /// </summary>
        /// <param name="dto">DTO con los datos del miembro a crear.</param>
        Task AddAsync(BoardMemberCreateDto dto);

        /// <summary>
        /// Añade varios miembros a un mismo tablero.
        /// </summary>
        /// <param name="dtos">Lista de DTOs de miembros a crear.</param>
        Task AddManyAsync(List<BoardMemberCreateDto> dtos);

        /// <summary>
        /// Actualiza el rol de un miembro en un tablero.
        /// </summary>
        /// <param name="dto">DTO con los datos de actualización.</param>
        /// <returns><c>true</c> si la actualización fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> UpdateAsync(BoardMemberUpdateDto dto);

        /// <summary>
        /// Elimina un miembro de un tablero.
        /// </summary>
        /// <param name="boardId">Identificador del tablero.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; de lo contrario, <c>false</c>.</returns>
        Task<bool> DeleteAsync(Guid boardId, Guid userId);
    }
}
