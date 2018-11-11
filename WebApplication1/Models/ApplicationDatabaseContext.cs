using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    /// <summary>
    /// A model containing all application data with a connection to an external store
    /// </summary>
    public class ApplicationDatabaseContext : DbContext
    {
        /// <summary>
        /// Create an instance of this ApplicationDatabaseContext
        /// </summary>
        /// <param name="options">Options for creating this model</param>
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options) { }

        /// <summary>
        /// A List of Key Binded TodoItems 
        /// </summary>
        public DbSet<TodoItemKeyContainer> TodoItems { get; set; }
        /// <summary>
        /// The entire collection of TodoItems
        /// </summary>
        public DbSet<TodoItem> TodoItem { get; set; }

    }
}