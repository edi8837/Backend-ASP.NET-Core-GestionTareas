using Microsoft.EntityFrameworkCore;
using WebApiTask.Models;

namespace WebApiTask.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }

        public DbSet<Models.Task> Tasks { get; set; }
    }
}
