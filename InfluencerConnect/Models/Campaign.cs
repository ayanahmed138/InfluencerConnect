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
        public int TargetAudienceId { get; set; }
        public TargetAudience TargetAudience { get; set; }
        public int CatagoryId { get; set; }
        public Category Category { get; set; }
        public int CampaignMessageId { get; set; }
        public CampaignMessage CampaignMessage { get; set; } 


        public bool IsDeleted { get; set; }
    }
}