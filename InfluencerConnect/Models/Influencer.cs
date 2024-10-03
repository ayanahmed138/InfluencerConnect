using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class Influencer
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name{ get; set; }
        public string ContactInfo { get; set; }
        public int MinCharge{ get; set; }
        public int MaxCharge{ get; set; }
        public int Limit { get; set; }
        public bool IsDeleted { get; set; }
        public List<Campaign> Campaigns { get; set; }
        public List<InfluencerContentType> InfluencerContentType { get; set; }

    }
}