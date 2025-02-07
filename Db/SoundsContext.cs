using Microsoft.EntityFrameworkCore;
using Sounds_New.Models;

namespace Sounds_New.Db
{
    public class SoundsContext(DbContextOptions<SoundsContext> options) : DbContext(options)
    {
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<User> Users => Set<User>();
    }
}