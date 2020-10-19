using Microsoft.EntityFrameworkCore;
using MusicHunterServer.Models;

namespace MusicHunterServer.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationRelation> ConversationRelations { get; set; }
        public DbSet<Participant> Participants { get; set; }

        public DbSet<TrackToUser> TrackUserRelations { get; set; }
        public DbSet<PlaylistToUser> PlaylistUserRelations { get; set; }

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
    }
}
