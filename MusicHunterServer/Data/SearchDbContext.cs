using Microsoft.EntityFrameworkCore;
using MusicHunterServer.Models;

namespace MusicHunterServer.Data
{
    public class SearchDbContext:DbContext
    {
        public SearchDbContext(DbContextOptions<SearchDbContext> options): base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

    }
}
