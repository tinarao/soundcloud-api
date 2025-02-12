using Microsoft.EntityFrameworkCore;
using Sounds_New.Models;

namespace Sounds_New.Db
{
    public class SoundsContext(DbContextOptions<SoundsContext> options) : DbContext(options)
    {
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Slug)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("DATETIME('now')");

            // 

            modelBuilder.Entity<Track>()
                .HasIndex(t => t.Slug)
                .IsUnique();
        }
    }
}