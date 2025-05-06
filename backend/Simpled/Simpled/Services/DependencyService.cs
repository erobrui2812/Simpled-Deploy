using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    public class DependencyService : IDependencyRepository
    {
        private readonly SimpledDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DependencyService(
            SimpledDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Dependency>> GetByBoardAsync(Guid boardId)
        {
            return await _context.Dependencies
                .Where(d => d.BoardId == boardId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Dependency> CreateAsync(Dependency dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));

            dependency.Id = Guid.NewGuid();
            _context.Dependencies.Add(dependency);
            await _context.SaveChangesAsync();
            return dependency;
        }
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Dependencies.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Dependencia con id {id} no encontrada.");

            _context.Dependencies.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
