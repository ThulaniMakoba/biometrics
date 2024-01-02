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

            modelBuilder.Entity<User>()
                .HasMany(a => a.Faces)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);         

            modelBuilder.Entity<FaceData>()
              .Property(f => f.Id)
              .ValueGeneratedOnAdd();

            modelBuilder.Entity<LogTransaction>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<FaceData> FaceData { get; set; }
        public DbSet<LogTransaction> LogTransaction { get; set; }
    }
}
