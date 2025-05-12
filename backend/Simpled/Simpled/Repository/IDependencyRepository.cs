
using Simpled.Models;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones para gestionar las dependencias entre tareas.
    /// </summary>
    public interface IDependencyRepository
    {
        /// <summary>
        /// Obtiene todas las dependencias asociadas a un tablero.
        /// </summary>
        /// <param name="boardId">Identificador del tablero.</param>
        /// <returns>Lista de entidades <see cref="Dependency"/>.</returns>
        Task<IEnumerable<Dependency>> GetByBoardAsync(Guid boardId);

        /// <summary>
        /// Crea una nueva dependencia entre tareas.
        /// </summary>
        /// <param name="dependency">Entidad <see cref="Dependency"/> a crear.</param>
        /// <returns>Entidad <see cref="Dependency"/> creada.</returns>
        Task<Dependency> CreateAsync(Dependency dependency);

        /// <summary>
        /// Elimina una dependencia por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la dependencia.</param>
        Task DeleteAsync(Guid id);
    }
}
