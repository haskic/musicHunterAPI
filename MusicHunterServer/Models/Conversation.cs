using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class Conversation
    {
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatorHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDuo { get; set; }


    }
}
