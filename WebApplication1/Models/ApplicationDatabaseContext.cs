using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ApplicationDatabaseContext : DbContext
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options) { }

        public DbSet<TodoItemKeyContainer> TodoItems { get; set; }

    }
}