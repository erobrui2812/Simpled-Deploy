
using Simpled.Dtos.Items;
using Simpled.Dtos.Subtasks;
using Simpled.Models;

namespace Simpled.Repository
{
    /// <summary>
    /// Define las operaciones de gestión de ítems (tareas) y sus subtareas.
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Obtiene todos los ítems.
        /// </summary>
        /// <returns>Secuencia de DTOs de ítems.</returns>
        Task<IEnumerable<ItemReadDto>> GetAllAsync();

        /// <summary>
        /// Obtiene un ítem por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del ítem.</param>
        /// <returns>DTO del ítem, o <c>null</c> si no existe.</returns>
        Task<ItemReadDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Crea un nuevo ítem.
        /// </summary>
        /// <param name="dto">DTO con los datos de creación del ítem.</param>
        /// <returns>DTO con los datos del ítem creado.</returns>
        Task<ItemReadDto> CreateAsync(ItemCreateDto dto);

        /// <summary>
        /// Actualiza los datos de un ítem existente.
        /// </summary>
        /// <param name="dto">DTO con los datos de actualización.</param>
        /// <returns><c>true</c> si la actualización fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> UpdateAsync(ItemUpdateDto dto);

        /// <summary>
        /// Cambia el estado de un ítem.
        /// </summary>
        /// <param name="id">Identificador del ítem.</param>
        /// <param name="status">Nuevo estado (pending, in-progress, completed o delayed).</param>
        /// <returns><c>true</c> si el cambio fue exitoso; en caso contrario, <c>false</c>.</returns>
        Task<bool> UpdateStatusAsync(Guid id, string status);

        /// <summary>
        /// Elimina un ítem por su identificador.
        /// </summary>
        /// <param name="id">Identificador del ítem a eliminar.</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Sube un archivo y lo asocia a un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <param name="file">Archivo a subir.</param>
        /// <returns>Entidad <c>Content</c> creada, o <c>null</c> si falla.</returns>
        Task<Content?> UploadFileAsync(Guid itemId, IFormFile file);

        /// <summary>
        /// Obtiene el identificador del tablero al que pertenece una columna.
        /// </summary>
        /// <param name="columnId">Identificador de la columna.</param>
        /// <returns>Identificador del tablero asociado.</returns>
        Task<Guid> GetBoardIdByColumnId(Guid columnId);

        /// <summary>
        /// Obtiene el identificador del tablero al que pertenece un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <returns>Identificador del tablero asociado.</returns>
        Task<Guid> GetBoardIdByItemId(Guid itemId);

        /// <summary>
        /// Obtiene las subtareas asociadas a un ítem.
        /// </summary>
        /// <param name="itemId">Identificador del ítem.</param>
        /// <returns>Secuencia de DTOs de subtareas.</returns>
        Task<IEnumerable<SubtaskDto>> GetSubtasksByItemIdAsync(Guid itemId);

        /// <summary>
        /// Crea una nueva subtarea para un ítem.
        /// </summary>
        /// <param name="dto">DTO con los datos de la subtarea.</param>
        /// <returns>DTO con los datos de la subtarea creada.</returns>
        Task<SubtaskDto> CreateSubtaskAsync(SubtaskCreateDto dto);

        /// <summary>
        /// Actualiza una subtarea existente.
        /// </summary>
        /// <param name="dto">DTO con los datos de actualización.</param>
        /// <returns><c>true</c> si la actualización fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> UpdateSubtaskAsync(SubtaskUpdateDto dto);

        /// <summary>
        /// Elimina una subtarea por su identificador.
        /// </summary>
        /// <param name="subtaskId">Identificador de la subtarea a eliminar.</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; en caso contrario, <c>false</c>.</returns>
        Task<bool> DeleteSubtaskAsync(Guid subtaskId);
    }
}
