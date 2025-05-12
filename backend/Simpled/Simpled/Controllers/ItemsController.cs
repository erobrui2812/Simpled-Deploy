using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Items;
using Simpled.Exception;
using Simpled.Helpers;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Dtos.ActivityLogs;

namespace Simpled.Controllers
{
    /// <summary>
    /// Gestiona las operaciones CRUD de ítems en un tablero Kanban,
    /// incluyendo registro de actividad en cada operación.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemService;
        private readonly IBoardMemberRepository _memberRepo;
        private readonly IActivityLogRepository _logRepo;

        /// <summary>
        /// Constructor que inyecta servicios de ítems, miembros de tablero y logs de actividad.
        /// </summary>
        public ItemsController(
            IItemRepository itemService,
            IBoardMemberRepository memberRepo,
            IActivityLogRepository logRepo)
        {
            _itemService = itemService;
            _memberRepo = memberRepo;
            _logRepo = logRepo;
        }

        /// <summary>
        /// Obtiene todos los ítems.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Obtiene un ítem por su ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ItemReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            try
            {
                var item = await _itemService.GetByIdAsync(id);
                return Ok(item);
            }
            catch (NotFoundException)
            {
                return NotFound("Ítem no encontrado.");
            }
        }

        /// <summary>
        /// Crea un nuevo ítem y registra la actividad correspondiente.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ItemReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateDto dto)
        {
            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
                return Forbid("No tienes permisos para crear ítems en este tablero.");

            if (dto.AssigneeId.HasValue &&
                !await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin" }, _memberRepo))
                return Forbid("Solo el admin puede asignar responsables.");

            var created = await _itemService.CreateAsync(dto);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Registrar creación con tipo estándar
            await _logRepo.AddAsync(new ActivityLog
            {
                Id = Guid.NewGuid(),
                ItemId = created.Id,
                UserId = userId,
                Action = ActivityType.Created.ToString(),
                Details = created.Title,
                Timestamp = DateTime.UtcNow
            });

            return CreatedAtAction(nameof(GetItem), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un ítem (campos, estado, responsable, fechas) y registra la actividad.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var boardId = await _itemService.GetBoardIdByColumnId(dto.ColumnId);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var member = await _memberRepo.GetByIdsAsync(boardId, userId);

            if (member == null)
                return Forbid("No eres miembro de este tablero.");

            var before = await _itemService.GetByIdAsync(id);

            // Admin: puede cambiar todos los campos
            if (member.Role.ToLower() == "admin")
            {
                await _itemService.UpdateAsync(dto);

                // Título
                if (!string.Equals(before.Title, dto.Title, StringComparison.Ordinal))
                {
                    await _logRepo.AddAsync(new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        ItemId = id,
                        UserId = userId,
                        Action = ActivityType.Updated.ToString(),
                        Field = "Title",
                        OldValue = before.Title,
                        NewValue = dto.Title,
                        Details = "Cambio de título",
                        Timestamp = DateTime.UtcNow
                    });
                }

                // Descripción
                if ((before.Description ?? string.Empty) != (dto.Description ?? string.Empty))
                {
                    await _logRepo.AddAsync(new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        ItemId = id,
                        UserId = userId,
                        Action = ActivityType.Updated.ToString(),
                        Field = "Description",
                        OldValue = before.Description,
                        NewValue = dto.Description,
                        Details = "Cambio de descripción",
                        Timestamp = DateTime.UtcNow
                    });
                }

                // Responsable
                if (before.AssigneeId != dto.AssigneeId)
                {
                    await _logRepo.AddAsync(new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        ItemId = id,
                        UserId = userId,
                        Action = ActivityType.Assigned.ToString(),
                        Field = "AssigneeId",
                        OldValue = before.AssigneeId?.ToString(),
                        NewValue = dto.AssigneeId?.ToString(),
                        Details = dto.AssigneeId.HasValue ? $"Asignado a {dto.AssigneeId}" : "Sin asignar",
                        Timestamp = DateTime.UtcNow
                    });
                }

                // Fecha de inicio
                if (before.StartDate != dto.StartDate)
                {
                    await _logRepo.AddAsync(new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        ItemId = id,
                        UserId = userId,
                        Action = ActivityType.Updated.ToString(),
                        Field = "StartDate",
                        OldValue = before.StartDate?.ToString("o"),
                        NewValue = dto.StartDate?.ToString("o"),
                        Details = "Cambio de fecha de inicio",
                        Timestamp = DateTime.UtcNow
                    });
                }

                // Fecha de vencimiento
                if (before.DueDate != dto.DueDate)
                {
                    await _logRepo.AddAsync(new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        ItemId = id,
                        UserId = userId,
                        Action = ActivityType.Updated.ToString(),
                        Field = "DueDate",
                        OldValue = before.DueDate?.ToString("o"),
                        NewValue = dto.DueDate?.ToString("o"),
                        Details = "Cambio de fecha de vencimiento",
                        Timestamp = DateTime.UtcNow
                    });
                }

                // Estado
                if (!string.Equals(before.Status, dto.Status, StringComparison.Ordinal))
                {
                    await _logRepo.AddAsync(new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        ItemId = id,
                        UserId = userId,
                        Action = ActivityType.StatusChanged.ToString(),
                        Field = "Status",
                        OldValue = before.Status,
                        NewValue = dto.Status,
                        Details = $"Estado cambiado a {dto.Status}",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return NoContent();
            }
            // Editor: solo puede cambiar estado si es responsable
            else if (member.Role.ToLower() == "editor" && dto.AssigneeId == userId)
            {
                await _itemService.UpdateStatusAsync(id, dto.Status);
                await _logRepo.AddAsync(new ActivityLog
                {
                    Id = Guid.NewGuid(),
                    ItemId = id,
                    UserId = userId,
                    Action = ActivityType.StatusChanged.ToString(),
                    Details = dto.Status,
                    Timestamp = DateTime.UtcNow
                });

                return NoContent();
            }
            else
            {
                return Forbid("No tienes permisos para modificar este ítem.");
            }
        }

        /// <summary>
        /// Elimina un ítem y registra la actividad de eliminación.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin" }, _memberRepo))
                return Forbid("No tienes permisos para eliminar este ítem.");

            try
            {
                await _itemService.DeleteAsync(id);
                await _logRepo.AddAsync(new ActivityLog
                {
                    Id = Guid.NewGuid(),
                    ItemId = id,
                    UserId = userId,
                    Action = ActivityType.Deleted.ToString(),
                    Details = "Tarea eliminada",
                    Timestamp = DateTime.UtcNow
                });

                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound("Ítem no encontrado.");
            }
        }

        /// <summary>
        /// Sube un archivo adjunto a un ítem y registra la actividad.
        /// </summary>
        [HttpPost("{id:guid}/upload")]
        [ProducesResponseType(typeof(Content), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var boardId = await _itemService.GetBoardIdByItemId(id);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, boardId, new[] { "admin", "editor" }, _memberRepo))
                return Forbid("No tienes permisos para subir archivos en este ítem.");

            var content = await _itemService.UploadFileAsync(id, file);
            if (content == null)
                return BadRequest("Error al subir archivo o ítem no encontrado.");

            await _logRepo.AddAsync(new ActivityLog
            {
                Id = Guid.NewGuid(),
                ItemId = id,
                UserId = userId,
                Action = ActivityType.FileUploaded.ToString(),
                Details = file.FileName,
                Timestamp = DateTime.UtcNow
            });

            return Ok(content);
        }
    }
}