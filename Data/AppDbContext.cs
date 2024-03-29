﻿using biometricService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace biometricService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<User>()
                .HasMany(a => a.FaceData)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            modelBuilder
                .Entity<FaceData>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<FaceData>()
                .Property(f => f.FaceBase64)
                .IsUnicode(false);

            modelBuilder
                .Entity<LogTransaction>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<ConfigSetting>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<FaceData> FaceData { get; set; }
        public DbSet<LogTransaction> LogTransaction { get; set; }
        public DbSet<ConfigSetting> ConfigSettings { get; set; }
    }
}
