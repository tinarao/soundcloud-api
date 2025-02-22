using Microsoft.EntityFrameworkCore;
using Sounds_New.Models;

namespace Sounds_New.Db
{
    public class SoundsContext(DbContextOptions<SoundsContext> options) : DbContext(options)
    {
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<SignedUrl> SignedUrls => Set<SignedUrl>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Comment> Comments => Set<Comment>();
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

            modelBuilder.Entity<Track>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("DATETIME('now')");

            //

            modelBuilder.Entity<Like>()
                .HasIndex(l => l.TrackId);

            modelBuilder.Entity<Like>()
                .HasIndex(l => l.UserId);

            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.TrackId, l.UserId })
                .IsUnique();

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Track)
                .WithMany(t => t.Likes)
                .HasForeignKey(l => l.TrackId)
                .IsRequired();

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.TrackId)
                .IsRequired();

            modelBuilder.Entity<Like>()
                .Property(l => l.CreatedAt)
                .HasDefaultValueSql("DATETIME('now')");

            //

            modelBuilder.Entity<Comment>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("DATETIME('now')");
        }
    }
}