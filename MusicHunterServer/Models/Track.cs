using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string HashUrl { get; set; }
        public int OwnerId { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public string Histogram { get; set; }
        [NotMapped]
        public string Type { get; set; }

    }
}
