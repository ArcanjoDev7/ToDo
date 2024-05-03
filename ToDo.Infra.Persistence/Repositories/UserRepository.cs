using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Models;
using ToDo.Infra.Persistence.Data;
using ToDo.Infra.Persistence.Repositories.Interfaces;

namespace ToDo.Infra.Persistence.Repositories
{
    public class UserRepository(AppDbContext context) : RepositoryBase<User>(context), IUserRepository
    {
        public async Task<bool> EmailExistAsync(string email)
        {
            return await Context.Users
                .AsNoTracking()
                .Select(x => x.Email)
                .AnyAsync(x => x == email);
        }

        public async Task<User?> GetAsNoTrackingAsync(string email)
        {
            return await Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetAsync(string email)
        {
            return await Context.Users
               .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
