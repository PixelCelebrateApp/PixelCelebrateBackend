using Microsoft.EntityFrameworkCore;

namespace PixelCelebrateBackend.Entities
{
    public class PixelCelebrateDbContext : DbContext
    {
        public PixelCelebrateDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Configuration> Configurations { get; set; }
    }
}