using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Columns;
using Simpled.Exception;
using Simpled.Hubs;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Validators;
namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión de columnas de tableros.
    /// Implementa IColumnRepository.
    /// </summary>
    public class ColumnService : IColumnRepository
    {
        private readonly SimpledDbContext _context;
        private readonly IHubContext<BoardHub> _hubContext;

        public ColumnService(SimpledDbContext context, IHubContext<BoardHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Obtiene todas las columnas de todos los tableros.
        /// </summary>
        /// <returns>Lista de columnas.</returns>
        public async Task<IEnumerable<BoardColumnReadDto>> GetAllAsync()
        {
            return await _context.BoardColumns
                .Select(c => new BoardColumnReadDto
                {
                    Id = c.Id,
                    BoardId = c.BoardId,
                    Title = c.Title,
                    Order = c.Order
                })
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una columna por su ID.
        /// </summary>
        /// <param name="id">ID de la columna.</param>
        /// <returns>DTO de la columna o excepción si no existe.</returns>
        public async Task<BoardColumnReadDto?> GetByIdAsync(Guid id)
        {
            var column = await _context.BoardColumns.FindAsync(id);
            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            return new BoardColumnReadDto
            {
                Id = column.Id,
                BoardId = column.BoardId,
                Title = column.Title,
                Order = column.Order
            };
        }

        /// <summary>
        /// Crea una nueva columna en un tablero.
        /// </summary>
        /// <param name="dto">Datos de la columna a crear.</param>
        /// <returns>DTO de la columna creada.</returns>
        public async Task<BoardColumnReadDto> CreateAsync(BoardColumnCreateDto dto)
        {
            var validator = new ColumnCreateValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ApiException(validationResult.Errors[0].ErrorMessage, 400);
            var newColumn = new BoardColumn
            {
                Id = Guid.NewGuid(),
                BoardId = dto.BoardId,
                Title = dto.Title,
                Order = dto.Order
            };

            _context.BoardColumns.Add(newColumn);
            await _context.SaveChangesAsync();

            var columnDto = new BoardColumnReadDto
            {
                Id = newColumn.Id,
                BoardId = newColumn.BoardId,
                Title = newColumn.Title,
                Order = newColumn.Order
            };

            
            await _hubContext.Clients
                .Group(dto.BoardId.ToString())
                .SendAsync("BoardUpdated", dto.BoardId.ToString(), "ColumnCreated", columnDto);

            return columnDto;
        }

        /// <summary>
        /// Actualiza los datos de una columna existente.
        /// </summary>
        /// <param name="dto">Datos actualizados de la columna.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        public async Task<bool> UpdateAsync(BoardColumnUpdateDto dto)
        {
            var validator = new ColumnUpdateValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ApiException(validationResult.Errors[0].ErrorMessage, 400);
            var column = await _context.BoardColumns.FindAsync(dto.Id);
            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            column.Title = dto.Title;
            column.Order = dto.Order;

            await _context.SaveChangesAsync();

          
            await _hubContext.Clients
                .Group(dto.BoardId.ToString())
                .SendAsync("BoardUpdated", dto.BoardId.ToString(), "ColumnUpdated", new
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Order = dto.Order
                });

            return true;
        }

        /// <summary>
        /// Elimina una columna por su ID, con opciones de cascada o mover ítems.
        /// </summary>
        /// <param name="id">ID de la columna.</param>
        /// <returns>True si la eliminación fue exitosa.</returns>
        public Task<bool> DeleteAsync(Guid id)
        {
           
            return DeleteAsync(id, cascadeItems: false, targetColumnId: null);
        }

        /// <summary>
        /// Elimina una columna con opciones avanzadas.
        /// </summary>
        /// <param name="columnId">ID de la columna.</param>
        /// <param name="cascadeItems">Si es true, elimina los ítems asociados.</param>
        /// <param name="targetColumnId">Columna destino para mover ítems (opcional).</param>
        /// <returns>True si la eliminación fue exitosa.</returns>
        public async Task<bool> DeleteAsync(Guid columnId, bool cascadeItems = false, Guid? targetColumnId = null)
        {
            var column = await _context.BoardColumns
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == columnId);

            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            var boardId = column.BoardId;

            if (column.Items.Any())
            {
                if (targetColumnId.HasValue)
                {
                    var target = await _context.BoardColumns.FindAsync(targetColumnId.Value);
                    if (target == null)
                        throw new NotFoundException("Columna destino no encontrada.");

                    foreach (var item in column.Items)
                        item.ColumnId = targetColumnId.Value;
                }
                else if (cascadeItems)
                {
                    _context.Items.RemoveRange(column.Items);
                }
                else
                {
                    throw new InvalidOperationException(
                        "La columna contiene tareas. Debes moverlas o usar cascadeItems=true.");
                }
            }

            _context.BoardColumns.Remove(column);
            await _context.SaveChangesAsync();

     
            await _hubContext.Clients
                .Group(boardId.ToString())
                .SendAsync("BoardUpdated", boardId.ToString(), "ColumnDeleted", new
                {
                    ColumnId = columnId
                });

            return true;
        }

        /// <summary>
        /// Obtiene el ID del tablero al que pertenece una columna.
        /// </summary>
        /// <param name="columnId">ID de la columna.</param>
        /// <returns>ID del tablero.</returns>
        public async Task<Guid> GetBoardIdByColumnId(Guid columnId)
        {
            var column = await _context.BoardColumns.FindAsync(columnId);
            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            return column.BoardId;
        }
    }
}
