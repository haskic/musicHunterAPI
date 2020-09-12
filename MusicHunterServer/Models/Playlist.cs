using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Hash { get; set; }
        public int OwnerId { get; set; }
        public string ImageUrl { get; set; }

        [NotMapped]
        [JsonProperty("tracks")]
        public IList<Track> Tracks;

    }
}
