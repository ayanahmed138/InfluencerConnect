using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class CampaignMessage
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Content { get; set; }
        public string ShortDiscription { get; set; }
        public string LongDiscription { get; set; }
        public bool IsDeleted { get; set; }
        public int TargetAudienceId { get; set; }
        public virtual TargetAudience TargetAudience { get; set; }
        public int ContentTypeId { get; set; }
        public virtual ContentType ContentType { get; set; }
        public int Budget { get; set; }


    }
}