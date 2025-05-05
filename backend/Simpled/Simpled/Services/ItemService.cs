using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        {
            return await _context.Items
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
        }

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
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                ColumnId = dto.ColumnId,
                Status = dto.Status,
                AssigneeId = dto.AssigneeId
            };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var result = new ItemReadDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                DueDate = item.DueDate,
                ColumnId = item.ColumnId,
                Status = item.Status,
                AssigneeId = item.AssigneeId
            };

            var boardId = (await GetBoardIdByColumnId(dto.ColumnId)).ToString();
            await _hubContext.Clients
                .Group(boardId)
                .SendAsync("BoardUpdated", boardId, "ItemCreated", result);

            return result;
        }

        public async Task<bool> UpdateAsync(ItemUpdateDto dto)
        {
            var item = await _context.Items.FindAsync(dto.Id)
                ?? throw new NotFoundException("Ítem no encontrado.");
            item.Title = dto.Title;
            item.Description = dto.Description;
            item.DueDate = dto.DueDate;
            item.ColumnId = dto.ColumnId;
            item.Status = dto.Status;
            item.AssigneeId = dto.AssigneeId;
            await _context.SaveChangesAsync();

            var boardId = (await GetBoardIdByItemId(dto.Id)).ToString();
            await _hubContext.Clients
                .Group(boardId)
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
            var item = await _context.Items.FindAsync(id)
                ?? throw new NotFoundException("Ítem no encontrado.");
            item.Status = status;
            await _context.SaveChangesAsync();

            var boardId = (await GetBoardIdByItemId(id)).ToString();
            await _hubContext.Clients
                .Group(boardId)
                .SendAsync("BoardUpdated", boardId, "ItemStatusChanged", new { id, status });

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id)
                ?? throw new NotFoundException("Ítem no encontrado.");
            var boardId = (await GetBoardIdByItemId(id)).ToString();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            await _hubContext.Clients
                .Group(boardId)
                .SendAsync("BoardUpdated", boardId, "ItemDeleted", new { id });

            return true;
        }

        public async Task<Content?> UploadFileAsync(Guid itemId, IFormFile file)
        {
            var item = await _context.Items.FindAsync(itemId)
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

            var boardId = (await GetBoardIdByItemId(itemId)).ToString();
            await _hubContext.Clients
                .Group(boardId)
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
            var column = await _context.BoardColumns.FindAsync(columnId)
                ?? throw new NotFoundException("Columna no encontrada.");
            return column.BoardId;
        }

        public async Task<Guid> GetBoardIdByItemId(Guid itemId)
        {
            var item = await _context.Items.FindAsync(itemId)
                ?? throw new NotFoundException("Ítem no encontrado.");
            var column = await _context.BoardColumns.FindAsync(item.ColumnId)
                ?? throw new NotFoundException("Columna del ítem no encontrada.");
            return column.BoardId;
        }
    }
}
