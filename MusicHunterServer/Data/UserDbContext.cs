using Microsoft.EntityFrameworkCore;
using MusicHunterServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Data
{
    public class UserDbContext: DbContext
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
        public DbSet<PlaylistToUser> PlaylistUserRelations{ get; set; }


    }
}
