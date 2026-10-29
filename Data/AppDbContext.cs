using Microsoft.EntityFrameworkCore;
using MyFirstApi.Entities;

namespace MyFirstApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

            
        }

        // tables that we have to create for sql db
        public DbSet<User> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }
    }
}
