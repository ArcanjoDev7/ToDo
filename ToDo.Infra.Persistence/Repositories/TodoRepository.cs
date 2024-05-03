using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Models;
using ToDo.Infra.Persistence.Data;
using ToDo.Infra.Persistence.Repositories.Interfaces;

namespace ToDo.Infra.Persistence.Repositories
{
    public class TodoRepository(AppDbContext context) : RepositoryBase<Todo>(context), ITodoRepository
    {
        public async Task<List<Todo>> GetAllAsync(Guid userId)
        {
            return await Context.Todos
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<Todo?> GetAsync(Guid id)
        {
            return await Context.Todos
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
