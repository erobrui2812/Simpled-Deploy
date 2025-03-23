using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Models.TrelloNotionClone.Models;
using Simpled.Dtos.Items;
using Simpled.Models;
using Microsoft.AspNetCore.Authorization;

namespace Simpled.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly SimpledDbContext _context;

        public ItemsController(SimpledDbContext context)
        {
            _context = context;
        }

        // GET: api/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemReadDto>>> GetAllItems()
        {
            var items = await _context.Items.Include(i => i.Contents).ToListAsync();
            var itemDtos = items.Select(i => new ItemReadDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                DueDate = i.DueDate,
                ColumnId = i.ColumnId
            }).ToList();

            return Ok(itemDtos);
        }

        // GET: api/items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemReadDto>> GetItem(Guid id)
        {
            var item = await _context.Items
                                     .Include(i => i.Contents)
                                     .FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
                return NotFound("Item not found.");

            var dto = new ItemReadDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                DueDate = item.DueDate,
                ColumnId = item.ColumnId
            };

            return Ok(dto);
        }

        // POST: api/items
        [HttpPost]
        public async Task<ActionResult<ItemReadDto>> CreateItem([FromBody] ItemCreateDto createDto)
        {
            var newItem = new Item
            {
                Id = Guid.NewGuid(),
                Title = createDto.Title,
                Description = createDto.Description,
                DueDate = createDto.DueDate,
                ColumnId = createDto.ColumnId
            };

            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();

            var readDto = new ItemReadDto
            {
                Id = newItem.Id,
                Title = newItem.Title,
                Description = newItem.Description,
                DueDate = newItem.DueDate,
                ColumnId = newItem.ColumnId
            };

            return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, readDto);
        }

        // PUT: api/items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID mismatch.");

            var existing = await _context.Items.FindAsync(id);
            if (existing == null)
                return NotFound("Item not found.");

            existing.Title = updateDto.Title;
            existing.Description = updateDto.Description;
            existing.DueDate = updateDto.DueDate;
            existing.ColumnId = updateDto.ColumnId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/items/{id}
        // Solo admin puede borrar items
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound("Item not found.");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/items/{id}/upload
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound("Item not found.");

            if (file != null && file.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var content = new Content
                {
                    Id = Guid.NewGuid(),
                    ItemId = id,
                    Type = "image",
                    Value = "/uploads/" + fileName
                };
                _context.Contents.Add(content);
                await _context.SaveChangesAsync();

                return Ok(content);
            }

            return BadRequest("File is invalid.");
        }
    }
}
