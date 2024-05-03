using ToDo.Domain.Models;

namespace ToDo.Infra.Persistence.Repositories.Interfaces
{
    public interface ITodoRepository : IRepositoryBase<Todo>
    {
        Task<List<Todo>> GetAllAsync(Guid userId);
        Task<Todo?> GetAsync(Guid id);
    }
}
