
using Simpled.Dtos.Teams;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones de lectura y escritura sobre equipos.
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// Obtiene todos los equipos en los que participa un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>Secuencia de DTOs de equipos.</returns>
        Task<IEnumerable<TeamReadDto>> GetAllByUserAsync(Guid userId);

        /// <summary>
        /// Obtiene un equipo por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del equipo.</param>
        /// <returns>DTO con la información del equipo, o <c>null</c> si no existe.</returns>
        Task<TeamReadDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Crea un nuevo equipo.
        /// </summary>
        /// <param name="dto">DTO con los datos para crear el equipo.</param>
        /// <param name="ownerId">Identificador del usuario que será propietario.</param>
        /// <returns>DTO con la información del equipo creado.</returns>
        Task<TeamReadDto> CreateAsync(TeamCreateDto dto, Guid ownerId);

        /// <summary>
        /// Actualiza los datos de un equipo existente.
        /// </summary>
        /// <param name="dto">DTO con los nuevos datos del equipo.</param>
        /// <param name="ownerId">Identificador del propietario (para validaciones).</param>
        /// <returns><c>true</c> si la actualización fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> UpdateAsync(TeamUpdateDto dto, Guid ownerId);

        /// <summary>
        /// Elimina un equipo.
        /// </summary>
        /// <param name="id">Identificador del equipo a eliminar.</param>
        /// <param name="ownerId">Identificador del propietario (para validaciones).</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> DeleteAsync(Guid id, Guid ownerId);
    }
}
