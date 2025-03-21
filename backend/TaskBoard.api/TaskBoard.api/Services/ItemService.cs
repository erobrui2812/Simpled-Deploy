using TaskBoard.api.Data;
using TaskBoard.api.Models;

namespace TaskBoard.api.Services
{
    public class ItemService
    {
        private readonly AppDbContext _context;

        public ItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Item> MoveItemAsync(Guid itemId, Guid toColumnId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null) throw new KeyNotFoundException("Ítem no encontrado");

            item.ColumnId = toColumnId;
            await _context.SaveChangesAsync();

            return item;
        }
    }
}