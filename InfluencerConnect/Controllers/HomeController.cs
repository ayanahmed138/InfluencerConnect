using InfluencerConnect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfluencerConnect.Controllers
{
    public class HomeController : BaseController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();
        public CampaignViewHelper campaignViewHelper = new CampaignViewHelper();
        public InfluencerListViewModel influencerListViewHelper = new InfluencerListViewModel();
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                var user = db.Users.Find(userId);
                if (user.IsInfluencer)
                {
                    ViewBag.IsInfluencer = true;
                }
                else
                {
                    ViewBag.IsInfluencer = false;
                }
            }
            else
            {
                ViewBag.IsInfluencer = null;
            }
                return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public PartialViewResult _TopCampaigns()
        {
            var allCampaigns = db.Campaigns.OrderByDescending(x=>x.CreatedOn).ToList().Take(8);
            var campaingsToSend = new List<CampaignViewHelper>();
            foreach (var campaign in allCampaigns)
            {
               campaingsToSend.Add(campaignViewHelper.ToCampaignViewModel(campaign));
            }

            return PartialView(campaingsToSend);
        }

        public PartialViewResult _InfluencerCategories()
        {

            return PartialView();
        }
       public PartialViewResult _TopInfluencers()
        {
            var influencers = db.Influencer.Where(x => x.IsDeleted == false).OrderBy(x => Guid.NewGuid()).Take(4).ToList();
            var influencersToSend = new List<InfluencerListViewModel>();

            foreach(var influecer in influencers)
            {
                influencersToSend.Add(influencerListViewHelper.ToInfluencerListViewModel(influecer));

            }

            return PartialView(influencersToSend);
        }
        

              
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}