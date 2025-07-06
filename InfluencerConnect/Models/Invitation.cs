using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int CampaignMsgId { get; set; }
        [ForeignKey("CampaignMsgId")]
        public virtual CampaignMessage CampaignMessage { get; set; }
        public string InfluencerId { get; set; }
        public string AgentId { get; set; }
        public DateTime CreateOn { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDeleted { get; set; }
    }
}