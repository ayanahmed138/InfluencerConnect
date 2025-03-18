using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace InfluencerConnect.Models
{
    public class CampaignViewHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public int CampaignId { get; set; }
        public string CreatedBy { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Content { get; set; }
        public string ShortDiscription { get; set; }
        public string LongDiscription { get; set; }
        public string TargetAudience { get; set; }
        public string ContentType { get; set; }
        public List<CampaignImages> Images { get; set; }
        public string Category { get; set; }

        public CampaignViewHelper ToCampaignViewModel(Campaign campaign)
        {
            var campaignMsg = db.CampaignMessages.Where(x=>x.Id==campaign.Id).FirstOrDefault();
            
           
            var campaignViewHelper = new CampaignViewHelper
            {
                CampaignId = campaign.Id,
                CreatedBy = campaign.CreatedBy,
                IsPrivate = campaign.IsPrivate,
                StartDate = campaignMsg.StartDate,
                EndDate = campaignMsg.EndDate,
                Content = campaignMsg.Content,
                ShortDiscription = campaignMsg.ShortDiscription,
                LongDiscription = campaignMsg.LongDiscription,
                TargetAudience = db.TargetAudience.Where(x => x.Id == campaignMsg.TargetAudienceId).Select(x => x.Name).FirstOrDefault(),
                ContentType = db.TargetAudience.Where(x => x.Id == campaignMsg.TargetAudienceId).Select(x => x.Name).FirstOrDefault(),
                Category = db.Categories.Where(x=>x.Id==x.Id).Select(x=>x.Name).FirstOrDefault(),
                Images = new List<CampaignImages>(),


            };
            var campaignImages = db.CampaignImages.Where(x => x.CampaignMsgId == campaign.Id).ToList();

            foreach (var image in campaignImages)
            {
                campaignViewHelper.Images.Add(image);
            }
            //if (campaignViewHelper.Images.Count < 1)
            //{
            //    campaignViewHelper.Images.Add(new CampaignImages()
            //    {
            //        IamgeId = 0,
            //        PostId = post.PostId,
            //        FileName = "EC-Logo.jpg",
            //        FilePath = "\\Images\\EC-Logo.jpg",
            //        IsDeleted = false
            //    });
            //}
            //campaignViewHelper.ImagesCount = postViewModel.Images.Count();

            return campaignViewHelper;
        }

    }

}