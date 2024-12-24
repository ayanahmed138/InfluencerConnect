using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class Messages
    {
        public int Id{ get; set; }
        public int ChatId { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsViewed { get; set; }
        public bool IsDeleted { get; set; }
    }
}