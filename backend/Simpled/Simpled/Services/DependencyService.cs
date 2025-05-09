using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión de dependencias entre ítems.
    /// Implementa IDependencyRepository.
    /// </summary>
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

        /// <summary>
        /// Obtiene todas las dependencias de un tablero.
        /// </summary>
        /// <param name="boardId">ID del tablero.</param>
        /// <returns>Lista de dependencias.</returns>
        public async Task<IEnumerable<Dependency>> GetByBoardAsync(Guid boardId)
        {
            return await _context.Dependencies
                .Where(d => d.BoardId == boardId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Crea una nueva dependencia.
        /// </summary>
        /// <param name="dependency">Entidad Dependency a crear.</param>
        /// <returns>Entidad creada.</returns>
        /// <exception cref="ArgumentNullException">Si la dependencia es null.</exception>
        public async Task<Dependency> CreateAsync(Dependency dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));

            dependency.Id = Guid.NewGuid();
            _context.Dependencies.Add(dependency);
            await _context.SaveChangesAsync();
            return dependency;
        }

        /// <summary>
        /// Elimina una dependencia por su ID.
        /// </summary>
        /// <param name="id">ID de la dependencia.</param>
        /// <exception cref="KeyNotFoundException">Si la dependencia no existe.</exception>
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
