using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class MarketingAgents
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string ContactInfo { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }

    }
}