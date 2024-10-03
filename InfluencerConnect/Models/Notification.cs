using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int CampaignMsgId { get; set; }
        public int InfluencerId { get; set; }
        public string text { get; set; }
        public string title { get; set; }
        public bool? isAccepted { get; set; }
        public bool isDeleted { get; set; }
    }
}