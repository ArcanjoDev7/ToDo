namespace ToDo.Infra.Persistence.Repositories.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveAsync();

    }
}
