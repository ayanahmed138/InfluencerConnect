using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy{ get; set; }
        
        public int CatagoryId { get; set; }
        public virtual Category Category { get; set; }
        public int CampaignMessageId { get; set; }
        public virtual CampaignMessage CampaignMessage { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDeleted { get; set; }
    }
}