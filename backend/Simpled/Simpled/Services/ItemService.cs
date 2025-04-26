using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Items;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exceptions;

namespace Simpled.Services
{
    public class ItemService : IItemRepository
    {
        private readonly SimpledDbContext _context;

        public ItemService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemReadDto>> GetAllAsync()
        {
            return await _context.Items
                .Include(i => i.Contents)
                .Select(i => new ItemReadDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DueDate = i.DueDate,
                    ColumnId = i.ColumnId
                })
                .ToListAsync();
        }

        public async Task<ItemReadDto?> GetByIdAsync(Guid id)
        {
            var item = await _context.Items.Include(i => i.Contents).FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
                throw new NotFoundException("Ítem no encontrado.");

            return new ItemReadDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                DueDate = item.DueDate,
                ColumnId = item.ColumnId
            };
        }

        public async Task<ItemReadDto> CreateAsync(ItemCreateDto dto)
        {
            var newItem = new Item
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                ColumnId = dto.ColumnId
            };

            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();

            return new ItemReadDto
            {
                Id = newItem.Id,
                Title = newItem.Title,
                Description = newItem.Description,
                DueDate = newItem.DueDate,
                ColumnId = newItem.ColumnId
            };
        }

        public async Task<bool> UpdateAsync(ItemUpdateDto dto)
        {
            var existing = await _context.Items.FindAsync(dto.Id);
            if (existing == null)
                throw new NotFoundException("Ítem no encontrado.");

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.DueDate = dto.DueDate;
            existing.ColumnId = dto.ColumnId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                throw new NotFoundException("Ítem no encontrado.");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Content?> UploadFileAsync(Guid itemId, IFormFile file)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                throw new NotFoundException("Ítem no encontrado para subir archivo.");

            if (file == null || file.Length == 0)
                throw new ApiException("El archivo es inválido o está vacío.", 400);

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var content = new Content
            {
                Id = Guid.NewGuid(),
                ItemId = itemId,
                Type = "image",
                Value = "/uploads/" + fileName
            };

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            return content;
        }


        public async Task<Guid> GetBoardIdByColumnId(Guid columnId)
        {
            var column = await _context.BoardColumns.FindAsync(columnId);
            if (column == null)
                throw new NotFoundException("Columna no encontrada.");

            return column.BoardId;
        }

        public async Task<Guid> GetBoardIdByItemId(Guid itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                throw new NotFoundException("Ítem no encontrado.");

            var column = await _context.BoardColumns.FindAsync(item.ColumnId);
            if (column == null)
                throw new NotFoundException("Columna del ítem no encontrada.");

            return column.BoardId;
        }

    }
}
