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
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public List<Campaign> Campaigns { get; set; }
        public virtual ICollection<InfluencerContentType> InfluencerContentTypes { get; set; }
        public string YoutubeLink { get; set; }
        public string TikTokLink { get; set; }
        public string InstagramLink { get; set; }
        public string AboutMe { get; set; }

    }
}