using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class InfluencerContentType
    {
        public int Id { get; set; }
        public string InfluencerId { get; set; }
        public virtual Influencer Influencer { get; set; }
        public int ContentTypeId { get; set; }
        public virtual ContentType ContentType { get; set; }
        public bool IsDeleted { get; set; }

    }
}