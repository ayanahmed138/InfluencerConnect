using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public List<Messages> Messages { get; set; }
    }
}