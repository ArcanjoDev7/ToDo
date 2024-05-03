using ToDo.Infra.Persistence.Data;
using ToDo.Infra.Persistence.Repositories.Interfaces;

namespace ToDo.Infra.Persistence.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly AppDbContext Context;
        public RepositoryBase(AppDbContext context)
        {
            Context = context;
        }
        public async Task CreateAsync(T entity)
        {

            await Context.AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await Task.Run(() => Context.Remove(entity));
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await Task.Run(() => Context.Update(entity));
        }

    }
}
