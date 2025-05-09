using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Models;

namespace Simpled.Repository
{
    /// <summary>
    /// Implementación de <see cref="IDependencyRepository"/> usando EF Core.
    /// </summary>
    public class DependencyRepository : IDependencyRepository
    {
        private readonly SimpledDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DependencyRepository"/>.
        /// </summary>
        /// <param name="context">Contexto de base de datos inyectado.</param>
        public DependencyRepository(SimpledDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Dependency>> GetByBoardAsync(Guid boardId)
        {
            return await _context.Dependencies
                .Where(d => d.BoardId == boardId)
                .ToListAsync();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        /// Si <paramref name="dependency"/> es <c>null</c>.
        /// </exception>
        public async Task<Dependency> CreateAsync(Dependency dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));

            _context.Dependencies.Add(dependency);
            await _context.SaveChangesAsync();
            return dependency;
        }

        /// <inheritdoc/>
        /// <exception cref="KeyNotFoundException">
        /// Si no se encuentra una dependencia con el <paramref name="id"/> dado.
        /// </exception>
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
