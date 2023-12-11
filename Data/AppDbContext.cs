using biometricService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace biometricService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
        }
        public DbSet<User> Users { get; set; }
    }
}
