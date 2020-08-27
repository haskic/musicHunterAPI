using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class Participant
    {   
        [Key]
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Hash { get; set; }

    }
}
