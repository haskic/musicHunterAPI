using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class TrackToUser
    {
        public int Id { get; set; }
        public string TrackHash { get; set; }
        public string UserHash { get; set; }
        
    }
}
