using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Models;

namespace ToDo.Infra.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Todo> Todos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
