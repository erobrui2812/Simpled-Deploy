using Microsoft.AspNetCore.Http;
using Simpled.Dtos.Items;
using Simpled.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simpled.Repository
{
    public interface IItemRepository
    {
        Task<IEnumerable<ItemReadDto>> GetAllAsync();
        Task<ItemReadDto?> GetByIdAsync(Guid id);
        Task<ItemReadDto> CreateAsync(ItemCreateDto dto);
        Task<bool> UpdateAsync(ItemUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<Content?> UploadFileAsync(Guid itemId, IFormFile file);

        Task<Guid> GetBoardIdByColumnId(Guid columnId);
        Task<Guid> GetBoardIdByItemId(Guid itemId);
    }
}
