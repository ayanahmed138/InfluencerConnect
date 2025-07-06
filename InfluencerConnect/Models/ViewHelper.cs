using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;

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
        public int Budget { get; set; }
        public string AgentName { get; set; }
        public string AgentProfilePic { get; set; }
        public int CampaignMsgId { get; set; }

        public CampaignViewHelper ToCampaignViewModel(Campaign campaign)
        {
            var campaignMsg = db.CampaignMessages.Where(x => x.Id == campaign.Id).FirstOrDefault();
            var user = db.Users.Where(x => x.Id == campaign.CreatedBy).FirstOrDefault();

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
                Budget = campaignMsg.Budget,
                TargetAudience = db.TargetAudience.Where(x => x.Id == campaignMsg.TargetAudienceId).Select(x => x.Name).FirstOrDefault(),
                ContentType = db.ContentType.Where(x => x.Id == campaignMsg.ContentTypeId).Select(x => x.Name).FirstOrDefault(),
                Category = db.Categories.Where(x => x.Id == campaign.CatagoryId).Select(x => x.Name).FirstOrDefault(),
                Images = new List<CampaignImages>(),
                AgentName = $"{user.FirstName} {user.LastName}",
                AgentProfilePic = user.ImagePath,
                CampaignMsgId = campaignMsg.Id,
                



            };
            var campaignImages = db.CampaignImages.Where(x => x.CampaignMsgId == campaign.CampaignMessageId).ToList();

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

    public class CategoryCountViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Count { get; set; }
    }

    public class InfluencerListViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int InfluencerId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string YoutTubeLink { get; set; }
        public string TikTokLink { get; set; }
        public string InstagramLink { get; set; }
        public string ImagePath { get; set; }
        public string UserId { get; set; }


        public InfluencerListViewModel ToInfluencerListViewModel(Influencer influencer)
        {

            var influencerListViewModel = new InfluencerListViewModel()
            {
                InfluencerId = influencer.Id,
                Name = influencer.Name,
                CategoryName = db.Categories.Where(x => x.Id == influencer.CategoryId).Select(x => x.Name).FirstOrDefault(),
                YoutTubeLink = influencer.YoutubeLink,
                TikTokLink = influencer.TikTokLink,
                InstagramLink = influencer.InstagramLink,
                ImagePath = db.Users.Where(x => x.Id == influencer.UserId).Select(x => x.ImagePath).FirstOrDefault(),
                UserId = influencer.UserId,
            };

            return influencerListViewModel;

        }

    }

    public class ContentTypeCountViewModel
    {
        public int ContentTypeId { get; set; }
        public string ContentTypeName { get; set; }
        public int Count { get; set; }
    }

    public class ChatsViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public int ChatId { get; set; }
        public string LastMessage { get; set; }
        public string CreatedOn { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserImg { get; set; }
        public string OtherUserId { get; set; }



        public ChatsViewModel toChatViewModel(Chat chat, string currentUserId)
        {
            var ChatMessages = db.Messages.Where(x => x.ChatId == chat.Id).ToList();
            var otherUserId = (chat.User1Id == currentUserId) ? chat.User2Id : chat.User1Id;
            var otherUser = db.Users.Find(otherUserId);
            var chatsViewModel = new ChatsViewModel
            {
                ChatId = chat.Id,
                CreatedOn = (ChatMessages.Count > 0) ? ChatMessages.LastOrDefault().CreatedOn.ToString("t") : "-",
                LastMessage = "",
                OtherUserId = otherUserId,
                OtherUserName = $"{otherUser.FirstName} {otherUser.LastName}",
                OtherUserImg = otherUser.ImagePath,
            };

            if (ChatMessages.Count > 0)
            {

                chatsViewModel.LastMessage = ChatMessages.LastOrDefault().Text;
            }
            else
            {
                chatsViewModel.LastMessage = "No Messages";
            }



            return chatsViewModel;
        }


    }

    public class UserInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImagePath { get; set; }

    }
        
    public class UserViewModel
    {
        public bool IsInfluencer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AboutMe { get; set; }
        public string PhoneNumber { get; set; }
        public int CategoryId { get; set; }
        public int MinCharge { get; set; }
        public int MaxCharge { get; set; }
        public int Limit { get; set; }
        public string YoutTubeLink { get; set; }
        public string TikTokLink { get; set; }
        public string InstagramLink { get; set; }
        public string ImagePath { get; set; }
        public string CompanyName { get; set; }
        public List<int> SelectedContentTypeIds { get; set; } = new List<int>();
        public List<SelectListItem> AllContentTypes { get; set; } = new List<SelectListItem>();

    }

    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public bool IsRead { get; set; }
        public string CreatedAgo { get; set; }
    }

    public class InvitationViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int InvitationId { get; set; }
        public int CampaignMsgId { get; set; }
        public string AgentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Budget { get; set; }
        public string CampaignTitle { get; set; }

        public InvitationViewModel ToInvitationViewModel(Invitation invitation)
        {
            var invitationViewModel = new InvitationViewModel()
            {
                InvitationId = invitation.Id,
                CampaignMsgId = invitation.CampaignMsgId,
                AgentName = db.MarketingAgents.Where(x => x.UserId == invitation.AgentId).FirstOrDefault().Name,
                StartDate = invitation.CampaignMessage.StartDate,
                EndDate = invitation.CampaignMessage.EndDate,
                Budget = invitation.CampaignMessage.Budget,
                CampaignTitle = invitation.CampaignMessage.Content,

            };

            return invitationViewModel;
        }

    }

    public class InfluencerCampaignsViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string CampaignTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CampaignMsgId { get; set; }

        public InfluencerCampaignsViewModel ToInfluencerCampaignViewModel(Invitation invite)
        {
            var influencerCampaignViewModel = new InfluencerCampaignsViewModel()
            {
                AgentId = invite.AgentId,
                CampaignTitle = invite.CampaignMessage.Content,
                StartDate = invite.CampaignMessage.StartDate,
                EndDate = invite.CampaignMessage.EndDate,
                CampaignMsgId = invite.CampaignMsgId,
                AgentName = db.MarketingAgents.Where(x => x.UserId == invite.AgentId).FirstOrDefault().Name,
            };


            return influencerCampaignViewModel;
        }


    }

    public class MyCampaignViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string CampaignTitle { get; set; }
        public int Budget { get; set; }
        public List<InfluencerChatViewModel> Influencers { get; set; } = new List<InfluencerChatViewModel>();
        public int CampaignMsgId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Visiblity { get; set; }
        public string Category { get; set; }
        public int CampaignId { get; set; }


    }

    public class InfluencerChatViewModel
    {
        public string InfluencerId { get; set; }
        public string InfluencerName { get; set; }
        public string ImagePath { get; set; }

    }

    public class MarketingAgentInvitationViewModel
    {
        public int InvitationId { get; set; }
        public string InfluencerName { get; set; }
        public string CampaignTitle { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
       
    }

    public class InfluencerViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public int InfluencerId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string TiktokLink { get; set; }
        public string YouTubeLink { get; set; }
        public string InstagramLink { get; set; }
        public string AboutMe { get; set; }
        public string PhoneNo { get; set; }
        public List<ContentType> ContentType { get; set; } = new List<ContentType>();
        public DateTime JoinedOn { get; set; }
        public string Email { get; set; }
        public int ActiveCampagins { get; set; }
        public int CompletedCampaigns { get; set; }
        public int AllCampagins { get; set; }
        public string ProfilePic { get; set; }
        public string Category { get; set; }

        public InfluencerViewModel ToInfluencerViewModel(Influencer influencer)
        {
            var user = db.Users.Where(x => x.Id == influencer.UserId).FirstOrDefault();
            var invitations = db.Invitation.Where(x => x.InfluencerId == influencer.UserId && x.IsDeleted == false && x.IsAccepted ==true).ToList();
            
            var contentTypes = db.InfluencerContentType.Where(x => x.InfluencerId == influencer.UserId).Select(x => x.ContentType).ToList();

            var influencerViewModel = new InfluencerViewModel()
            {
                InfluencerId = influencer.Id,
                UserId = influencer.UserId,
                Name = influencer.Name,
                TiktokLink = influencer.TikTokLink,
                YouTubeLink = influencer.YoutubeLink,
                InstagramLink = influencer.InstagramLink,
                AboutMe = influencer.AboutMe,
                JoinedOn = user.JoinedOn,
                PhoneNo = influencer.ContactInfo,
                Email = user.Email,
                ProfilePic = user.ImagePath,
                Category = db.Categories.Where(x => x.Id == influencer.CategoryId).FirstOrDefault().Name,
                AllCampagins = invitations.Count,
                ActiveCampagins = invitations.Where(x=>x.CampaignMessage.StartDate<DateTime.Now && x.CampaignMessage.EndDate>DateTime.Now).Count(),
                CompletedCampaigns = invitations.Where(x=>x.CampaignMessage.EndDate < DateTime.Now).Count(),
                ContentType = contentTypes,



            };

            return influencerViewModel;
        }

    }

}