using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Items;
using Simpled.Exception;
using Simpled.Hubs;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    public class ItemService : IItemRepository
    {
        private readonly SimpledDbContext _context;
        private readonly IHubContext<BoardHub> _hubContext;

        public ItemService(SimpledDbContext context, IHubContext<BoardHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<ItemReadDto>> GetAllAsync()
            => await _context.Items
                .Select(i => new ItemReadDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DueDate = i.DueDate,
                    ColumnId = i.ColumnId,
                    Status = i.Status,
                    AssigneeId = i.AssigneeId
                })
                .ToListAsync();

        public async Task<ItemReadDto?> GetByIdAsync(Guid id)
        {
            var i = await _context.Items.FindAsync(id)
                ?? throw new NotFoundException("Ítem no encontrado.");
            return new ItemReadDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                DueDate = i.DueDate,
                ColumnId = i.ColumnId,
                Status = i.Status,
                AssigneeId = i.AssigneeId
            };
        }

        public async Task<ItemReadDto> CreateAsync(ItemCreateDto dto)
        {
            var i = new Item
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                ColumnId = dto.ColumnId,
                Status = dto.Status,
                AssigneeId = dto.AssigneeId
            };
            _context.Items.Add(i);
            await _context.SaveChangesAsync();

            var result = new ItemReadDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                DueDate = i.DueDate,
                ColumnId = i.ColumnId,
                Status = i.Status,
                AssigneeId = i.AssigneeId
            };

            var boardId = await GetBoardIdByColumnId(dto.ColumnId);
            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("BoardUpdated", boardId, "ItemCreated", result);

            return result;
        }

        public async Task<bool> UpdateAsync(ItemUpdateDto dto)
        {
            var i = await _context.Items.FindAsync(dto.Id)
                ?? throw new NotFoundException("Ítem no encontrado.");
            i.Title = dto.Title;
            i.Description = dto.Description;
            i.DueDate = dto.DueDate;
            i.ColumnId = dto.ColumnId;
            i.Status = dto.Status;
            i.AssigneeId = dto.AssigneeId;
            await _context.SaveChangesAsync();

            var boardId = await GetBoardIdByItemId(dto.Id);
            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("BoardUpdated", boardId, "ItemUpdated", new
                {
                    dto.Id,
                    dto.Title,
                    dto.Description,
                    dto.DueDate,
                    dto.ColumnId,
                    dto.Status,
                    dto.AssigneeId
                });

            return true;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            var i = await _context.Items.FindAsync(id)
                ?? throw new NotFoundException("Ítem no encontrado.");
            i.Status = status;
            await _context.SaveChangesAsync();

            var boardId = await GetBoardIdByItemId(id);
            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("BoardUpdated", boardId, "ItemStatusChanged", new { id, status });

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var i = await _context.Items.FindAsync(id)
                ?? throw new NotFoundException("Ítem no encontrado.");
            var boardId = await GetBoardIdByItemId(id);

            _context.Items.Remove(i);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("BoardUpdated", boardId, "ItemDeleted", new { id });

            return true;
        }

        public async Task<Content?> UploadFileAsync(Guid itemId, IFormFile file)
        {
            var i = await _context.Items.FindAsync(itemId)
                ?? throw new NotFoundException("Ítem no encontrado para subir archivo.");
            if (file == null || file.Length == 0)
                throw new ApiException("El archivo es inválido o está vacío.", 400);

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploads);
            var filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(uploads, filename);
            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            var content = new Content
            {
                Id = Guid.NewGuid(),
                ItemId = itemId,
                Type = "image",
                Value = "/uploads/" + filename
            };
            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            var boardId = await GetBoardIdByItemId(itemId);
            await _hubContext.Clients.Group(boardId.ToString())
                .SendAsync("BoardUpdated", boardId, "ItemFileUploaded", new
                {
                    itemId,
                    contentId = content.Id,
                    content.Value
                });

            return content;
        }

        public async Task<Guid> GetBoardIdByColumnId(Guid columnId)
        {
            var c = await _context.BoardColumns.FindAsync(columnId)
                ?? throw new NotFoundException("Columna no encontrada.");
            return c.BoardId;
        }

        public async Task<Guid> GetBoardIdByItemId(Guid itemId)
        {
            var i = await _context.Items.FindAsync(itemId)
                ?? throw new NotFoundException("Ítem no encontrado.");
            var c = await _context.BoardColumns.FindAsync(i.ColumnId)
                ?? throw new NotFoundException("Columna del ítem no encontrada.");
            return c.BoardId;
        }
    }
}
