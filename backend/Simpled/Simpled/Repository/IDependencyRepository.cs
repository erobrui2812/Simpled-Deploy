using Simpled.Models;

namespace Simpled.Repository
{
    public interface IDependencyRepository
    {
        Task<IEnumerable<Dependency>> GetByBoardAsync(Guid boardId);

        Task<Dependency> CreateAsync(Dependency dependency);

        Task DeleteAsync(Guid id);
    }
}