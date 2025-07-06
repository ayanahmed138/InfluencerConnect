using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using InfluencerConnect.Models;
using Microsoft.AspNet.Identity;

namespace InfluencerConnect.Controllers
{
    public class MarketingAgentsController : BaseController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        InfluencerListViewModel influencerListViewHelper = new InfluencerListViewModel();

        // GET: MarketingAgents
        public ActionResult Index()
        {
            return View(db.MarketingAgents.ToList());
        }

        public PartialViewResult _MarketingAgentDashboard()
        {
            var currentUserId = User.Identity.GetUserId();

            var campaignsQuery = db.Campaigns.Where(x => x.CreatedBy == currentUserId && !x.IsDeleted);

            // Total campaigns (before any date filters)
            ViewBag.TotalCampaigns = campaignsQuery.Count();

            // Active = now between start and end date
            ViewBag.ActiveCampaigns = campaignsQuery.Count(x =>
                x.CampaignMessage.StartDate <= DateTime.Now &&
                x.CampaignMessage.EndDate >= DateTime.Now
            );

            // Upcoming = starts in future
            ViewBag.UpcomingCampaigns = campaignsQuery.Count(x =>
                x.CampaignMessage.StartDate > DateTime.Now
            );

            // Completed = ended already
            ViewBag.CompletedCampaigns = campaignsQuery.Count(x =>
                x.CampaignMessage.EndDate < DateTime.Now
            );


            return PartialView();
        }

        public PartialViewResult _MyCampaigns()
        {
            var currentUserId = User.Identity.GetUserId();

            var campaigns = db.Campaigns
                              .Where(x => x.CreatedBy == currentUserId && x.IsDeleted==false).OrderByDescending(x=>x.CreatedOn)
                              .ToList();

            var campaignsToSend = new List<MyCampaignViewModel>();

            foreach (var campaign in campaigns)
            {
                // ✅ Get accepted invitations only
                var acceptedInvites = db.Invitation
                                        .Where(i => i.CampaignMsgId == campaign.Id && i.IsAccepted == true)
                                        .ToList();

                var influencerIds = acceptedInvites
                                    .Where(i => !string.IsNullOrEmpty(i.InfluencerId))
                                    .Select(i => i.InfluencerId)
                                    .ToList();

                
                var influencers = new List<InfluencerChatViewModel>();

                if (influencerIds.Any())
                {
                    influencers = db.Users
                        .Where(u => influencerIds.Contains(u.Id))
                        .Select(u => new InfluencerChatViewModel
                        {
                            InfluencerId = u.Id,
                            InfluencerName = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                            ImagePath = string.IsNullOrEmpty(u.ImagePath) ? "/Content/default-avatar.png" : u.ImagePath
                        })
                        .ToList();
                }

                
                var campaignVm = new MyCampaignViewModel()
                {
                    CampaignId = campaign.Id,
                    CampaignMsgId = campaign.CampaignMessageId,
                    CampaignTitle = campaign.CampaignMessage?.Content ?? "Untitled",
                    Budget = campaign.CampaignMessage?.Budget ?? 0,
                    StartDate = campaign.CampaignMessage?.StartDate ?? DateTime.MinValue,
                    EndDate = campaign.CampaignMessage?.EndDate ?? DateTime.MinValue,
                    Visiblity = campaign.IsPrivate,
                    Category = db.Categories.Where(x=>x.Id==campaign.CatagoryId).FirstOrDefault().Name,
                    Influencers = influencers
                };

                campaignsToSend.Add(campaignVm);
            }

            return PartialView(campaignsToSend);
        }

        public PartialViewResult _InvitationStatus()
        {

            var currentUserId = User.Identity.GetUserId();

            var invitationStatusList = new List<MarketingAgentInvitationViewModel>();

            var myinvites = db.Invitation.Where(x => x.AgentId == currentUserId).ToList();

            foreach(var invite in myinvites)
            {
                var myinvite = new MarketingAgentInvitationViewModel()
                {
                    CampaignTitle = invite.CampaignMessage.Content,
                    InfluencerName = db.Influencer.Where(x=>x.UserId==invite.InfluencerId).FirstOrDefault().Name,
                    IsAccepted = invite.IsAccepted,
                    IsDeleted = invite.IsDeleted,
                    CreatedOn = invite.CreateOn,
                    InvitationId = invite.Id,

                };

                invitationStatusList.Add(myinvite);
            }


            return PartialView(invitationStatusList);
        }

        public PartialViewResult _RecommendedInfluencers()
        {
            var userId = User.Identity.GetUserId(); // or however you store agent ID
            var campaigns = db.Campaigns
                .Where(c => c.CreatedBy == userId)
                .ToList();

            List<Influencer> influencers;

            if (campaigns.Any())
            {
                // Group by category and find most used
                var topCategoryId = campaigns
                    .GroupBy(c => c.CatagoryId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                // Now get influencers from that category
                influencers = db.Influencer
            .Where(i => i.CategoryId == topCategoryId)
            .Take(4)
            .ToList();

            }
            else
            {
                // No campaigns? Show random influencers
                influencers = db.Influencer
                    .OrderBy(i => Guid.NewGuid())
                    .Take(4)
                    .ToList();
            }
            var influencersToSend = new List<InfluencerListViewModel>();

            foreach (var influencer in influencers)
            {
                influencersToSend.Add(influencerListViewHelper.ToInfluencerListViewModel(influencer));
            }

            return PartialView(influencersToSend);
        }

        // GET: MarketingAgents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarketingAgents marketingAgents = db.MarketingAgents.Find(id);
            if (marketingAgents == null)
            {
                return HttpNotFound();
            }
            return View(marketingAgents);
        }

        // GET: MarketingAgents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MarketingAgents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Name,Company,ContactInfo,IsDeleted,IsApproved")] MarketingAgents marketingAgents)
        {
            if (ModelState.IsValid)
            {
                db.MarketingAgents.Add(marketingAgents);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(marketingAgents);
        }

        // GET: MarketingAgents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarketingAgents marketingAgents = db.MarketingAgents.Find(id);
            if (marketingAgents == null)
            {
                return HttpNotFound();
            }
            return View(marketingAgents);
        }

        // POST: MarketingAgents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Name,Company,ContactInfo,IsDeleted,IsApproved")] MarketingAgents marketingAgents)
        {
            if (ModelState.IsValid)
            {
                db.Entry(marketingAgents).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(marketingAgents);
        }

        // GET: MarketingAgents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarketingAgents marketingAgents = db.MarketingAgents.Find(id);
            if (marketingAgents == null)
            {
                return HttpNotFound();
            }
            return View(marketingAgents);
        }

        // POST: MarketingAgents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MarketingAgents marketingAgents = db.MarketingAgents.Find(id);
            db.MarketingAgents.Remove(marketingAgents);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
