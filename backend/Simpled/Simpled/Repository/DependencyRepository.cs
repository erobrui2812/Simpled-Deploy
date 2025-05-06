using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Models;

namespace Simpled.Repository
{
    public class DependencyRepository : IDependencyRepository
    {
        private readonly SimpledDbContext _context;

        public DependencyRepository(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dependency>> GetByBoardAsync(Guid boardId)
        {
            return await _context.Dependencies
                .Where(d => d.BoardId == boardId)
                .ToListAsync();
        }

        public async Task<Dependency> CreateAsync(Dependency dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));

            _context.Dependencies.Add(dependency);
            await _context.SaveChangesAsync();
            return dependency;
        }

        public async Task DeleteAsync(Guid id)
        {
            var dep = await _context.Dependencies.FindAsync(id);
            if (dep == null)
                throw new KeyNotFoundException($"No se encontró la dependencia con id {id}");

            _context.Dependencies.Remove(dep);
            await _context.SaveChangesAsync();
        }
    }
}
