using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }
        public int ConversationId { get; set; }
        public string Hash { get; set; }

    }
}
