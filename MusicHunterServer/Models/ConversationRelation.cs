using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicHunterServer.Models
{
    public class ConversationRelation
    {
        [Key]
        public int RelationId { get; set; }
        public int ConversationId { get; set; }
        public string UserHash { get; set; }

    }
}
