using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class CampaignImages
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int CampaignMsgId { get; set; }
        public virtual CampaignMessage CampaignMessage { get; set; }
        public bool IsDeleted { get; set; }
    }
}