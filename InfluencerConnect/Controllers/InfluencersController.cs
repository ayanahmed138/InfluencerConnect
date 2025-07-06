using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InfluencerConnect.Models;
using Microsoft.AspNet.Identity;

namespace InfluencerConnect.Controllers
{
    public class InfluencersController : BaseController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        public InfluencerListViewModel influencerListViewHelper = new InfluencerListViewModel();
        public InvitationViewModel invitationViewHelper = new InvitationViewModel();
        public InfluencerCampaignsViewModel influencerCampaignViewHelper = new InfluencerCampaignsViewModel();
        public InfluencerViewModel influencerViewHelper = new InfluencerViewModel();
        public CampaignViewHelper campaignViewHelper = new CampaignViewHelper();

        // GET: Influencers
        public ActionResult Index(int? campaignMsgId, string mode = "Browse")
        {
            ViewBag.mode = mode;
            ViewBag.campaignMsgId = campaignMsgId;

            if (campaignMsgId != null)
            {
                var campaign = db.Campaigns.Where(x => x.CampaignMessageId == campaignMsgId).FirstOrDefault();
                if (campaign != null)
                {
                    ViewBag.categoryId = campaign.CatagoryId;
                    ViewBag.contentTypeId = campaign.CampaignMessage.ContentTypeId;

                }
            }

            var categoryCounts = db.Categories
              .Select(cat => new CategoryCountViewModel
              {
                  CategoryId = cat.Id,
                  CategoryName = cat.Name,
                  Count = db.Influencer.Count(c => c.CategoryId == cat.Id)
              })
              .ToList();

            ViewBag.CategoryCounts = categoryCounts;

            var contentTypeCounts = db.ContentType
                .Select(ct => new ContentTypeCountViewModel
                {
                    ContentTypeId = ct.Id,
                    ContentTypeName = ct.Name,
                    Count = db.InfluencerContentType.Count(ict => ict.ContentTypeId == ct.Id)
                })
            .ToList();

            ViewBag.ContentTypeCounts = contentTypeCounts;

            var influencer = db.Influencer.Include(i => i.Category);
            return View(influencer.ToList());
        }

        public PartialViewResult _InfluencerDashboard()
        {
            var currentUserId = User.Identity.GetUserId();
            var invitation = db.Invitation.Where(x => x.InfluencerId == currentUserId && x.IsAccepted == true).ToList();
            ViewBag.TotalCampaigns = invitation.Count;
            ViewBag.TotalCompleted = invitation.Where(x => x.CampaignMessage.EndDate < DateTime.Now).Count();
            ViewBag.ActiveCampaigns = invitation.Where(x => x.CampaignMessage.StartDate <= DateTime.Now && x.CampaignMessage.EndDate > DateTime.Now).Count();
            ViewBag.PendngInvitations = db.Invitation.Where(x => x.InfluencerId == currentUserId && x.IsAccepted == false && x.IsDeleted == false).Count();


            return PartialView();
        }

        public PartialViewResult _InvitationsPartial()
        {
            var currentUserId = User.Identity.GetUserId();
            var invitations = db.Invitation.Where(x => x.InfluencerId == currentUserId && x.IsAccepted == false && x.IsDeleted == false).ToList();
            var invitationsToSend = new List<InvitationViewModel>();

            foreach (var invite in invitations)
            {
                invitationsToSend.Add(invitationViewHelper.ToInvitationViewModel(invite));

            }

            return PartialView(invitationsToSend);
        }

        public PartialViewResult _MyCampaignPartial()
        {
            var currentUserId = User.Identity.GetUserId();
            var invitations = db.Invitation.Where(x => x.InfluencerId == currentUserId && x.IsAccepted == true && x.IsDeleted == false).ToList();
            var influencerCampaignsToSend = new List<InfluencerCampaignsViewModel>();

            foreach (var invite in invitations)
            {
                influencerCampaignsToSend.Add(influencerCampaignViewHelper.ToInfluencerCampaignViewModel(invite));
            }

            return PartialView(influencerCampaignsToSend);
        }

        public PartialViewResult _RecommendedCampaigns()
        {
            var currentUserId = User.Identity.GetUserId();

            int influencerCategoryId = db.Influencer.Where(x => x.UserId == currentUserId).FirstOrDefault().CategoryId;

            var campaigns = db.Campaigns.Where(x => x.CatagoryId == influencerCategoryId && x.IsDeleted == false)
                .OrderBy(x => x.CampaignMessage.StartDate)
                .Take(4).ToList();

           if(campaigns.Count()==0)
            {
                campaigns = db.Campaigns.Where(x => x.CampaignMessage.StartDate > DateTime.Now && x.IsDeleted == false).OrderBy(c => Guid.NewGuid()).Take(4).ToList();

            }

            var recommendedCampaigns = new List<CampaignViewHelper>();

            foreach (var campaign in campaigns)
            {

                recommendedCampaigns.Add(campaignViewHelper.ToCampaignViewModel(campaign));
            }



            return PartialView(recommendedCampaigns);
        }

        public PartialViewResult _SearchInfluencer(int page = 1, int pageSize = 9)
        {
            var allinfluencers = db.Influencer
                .OrderByDescending(i => i.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var influencersToSend = allinfluencers
                .Select(i => influencerListViewHelper.ToInfluencerListViewModel(i))
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)db.Influencer.Count() / pageSize);

            return PartialView(influencersToSend);
        }

        public ActionResult SearchInfluencers(List<int> categories, List<int> contentTypes, int? minPrice, int? maxPrice, string keyword, string mode = "Browse", int page = 1, int pageSize = 9)
        {
            ViewData["mode"] = mode;

            var influencers = db.Influencer.AsQueryable();

            // Filter by category
            if (categories != null && categories.Any())
            {
                influencers = influencers.Where(i => categories.Contains(i.CategoryId));
            }

            // Filter by content types
            if (contentTypes != null && contentTypes.Any())
            {
                var influencerUserIdsWithContentTypes = db.InfluencerContentType
                    .Where(ct => contentTypes.Contains(ct.ContentTypeId))
                    .Select(ct => ct.InfluencerId)
                    .Distinct()
                    .ToList();

                influencers = influencers.Where(i => influencerUserIdsWithContentTypes.Contains(i.UserId));
            }

            // Filter by price
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                influencers = influencers.Where(i =>
                    i.MinCharge <= maxPrice.Value && i.MaxCharge >= minPrice.Value);
            }
            else if (minPrice.HasValue)
            {
                influencers = influencers.Where(i => i.MaxCharge >= minPrice.Value);
            }
            else if (maxPrice.HasValue)
            {
                influencers = influencers.Where(i => i.MinCharge <= maxPrice.Value);
            }

            // Keyword filter
            if (!string.IsNullOrEmpty(keyword))
            {
                influencers = influencers.Where(i => i.Name.Contains(keyword));
            }

            // Pagination
            var totalCount = influencers.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedInfluencers = influencers
                .OrderByDescending(i => i.UserId) // Or by Name or some other field
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var influencerList = pagedInfluencers
                .Select(influencer => influencerListViewHelper.ToInfluencerListViewModel(influencer))
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return PartialView("_SearchInfluencer", influencerList);
        }


        // GET: Influencers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Influencer influencer = db.Influencer.Find(id);
            var influencerToSend = influencerViewHelper.ToInfluencerViewModel(influencer);

            if (influencer == null)
            {
                return HttpNotFound();
            }
            return View(influencerToSend);
        }

        public PartialViewResult _InfluencerPastCampagins(string influencerId)
        {
            var campaignMsgIds = db.Invitation.Where(x => x.InfluencerId == influencerId && x.IsAccepted && !x.IsDeleted)
            .OrderByDescending(x => x.CreateOn).Take(6).Select(x => x.CampaignMsgId).ToList();

            var campaigns = db.Campaigns
                .Where(c => campaignMsgIds.Contains(c.CampaignMessageId))
                .ToList();

            var campaignsToSend = campaigns
                .Select(c => campaignViewHelper.ToCampaignViewModel(c))
                .ToList();


            return PartialView(campaignsToSend);
        }

        // GET: Influencers/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Influencers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Name,ContactInfo,MinCharge,MaxCharge,Limit,IsDeleted,CategoryId")] Influencer influencer)
        {
            if (ModelState.IsValid)
            {
                db.Influencer.Add(influencer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", influencer.CategoryId);
            return View(influencer);
        }

        // GET: Influencers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Influencer influencer = db.Influencer.Find(id);
            if (influencer == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", influencer.CategoryId);
            return View(influencer);
        }

        // POST: Influencers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Name,ContactInfo,MinCharge,MaxCharge,Limit,IsDeleted,CategoryId")] Influencer influencer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(influencer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", influencer.CategoryId);
            return View(influencer);
        }

        // GET: Influencers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Influencer influencer = db.Influencer.Find(id);
            if (influencer == null)
            {
                return HttpNotFound();
            }
            return View(influencer);
        }

        // POST: Influencers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Influencer influencer = db.Influencer.Find(id);
            db.Influencer.Remove(influencer);
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
