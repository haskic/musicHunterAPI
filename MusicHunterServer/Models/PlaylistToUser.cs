using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class PlaylistToUser
    {
        public int Id { get; set; }
        public string  PlaylistHash { get; set; }
        public string UserHash { get; set; }
    }
}
