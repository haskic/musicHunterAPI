using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class PlaylistRelation
    {
        public int Id { get; set; }
        public string TrackHashUrl { get; set; }
        public string PlaylistHash { get; set; }

    }
}
