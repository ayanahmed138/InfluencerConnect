using InfluencerConnect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfluencerConnect.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public CampaignViewHelper campaignViewHelper = new CampaignViewHelper();

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
            var allCampaigns = db.Campaigns.OrderByDescending(x=>x.CreatedOn).ToList().Take(25);
            var campaingsToSend = new List<CampaignViewHelper>();
            foreach (var campaign in allCampaigns)
            {
               campaingsToSend.Add(campaignViewHelper.ToCampaignViewModel(campaign));
            }

            return PartialView(campaingsToSend);
        }
        public ActionResult InfluencerSearch() { 
        


            return View();
        }
        
       public PartialViewResult _InfluencerDashboard()
        {

            return PartialView();
        }
        public PartialViewResult _MarketingAgentDashboard()
        {

            return PartialView();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}