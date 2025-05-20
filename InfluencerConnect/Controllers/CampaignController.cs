using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using InfluencerConnect.Models;
using Microsoft.AspNet.Identity;

namespace InfluencerConnect.Controllers
{

    public class CampaignController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public CampaignViewHelper campaignViewHelper = new CampaignViewHelper();


        // GET: Campaign
        public ActionResult Index()
        {
            
            var categoryCounts = db.Categories
            .Select(cat => new CategoryCountViewModel
            {
                CategoryId = cat.Id,
                CategoryName = cat.Name,
                Count = db.Campaigns.Count(c => c.CatagoryId == cat.Id)
            })
            .ToList();

            ViewBag.CategoryCounts = categoryCounts;
            return View();
        }

        public PartialViewResult _CampaignPartialView()
        {
            var campaigns = db.Campaigns.ToList();
            var campaignsToSend = new List<CampaignViewHelper>();

            foreach (var campaign in campaigns)
            {
                campaignsToSend.Add(campaignViewHelper.ToCampaignViewModel(campaign));
            }


            return PartialView(campaignsToSend);
        }

        public ActionResult SearchCampaigns(List<int> categories, string keyword)
        {
            var query = db.Campaigns
                        .Include(c => c.CampaignMessage)
                        .AsQueryable();

            if (categories != null && categories.Any())
                query = query.Where(c => categories.Contains(c.CatagoryId));



            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(c => c.CampaignMessage.Content.ToLower().Contains(keyword) ||
                                         c.CampaignMessage.ShortDiscription.ToLower().Contains(keyword) ||
                                         c.CampaignMessage.LongDiscription.ToLower().Contains(keyword));
            }
            var campaigns = query.ToList();
            var campaignsToSend = new List<CampaignViewHelper>();

            foreach (var campaign in campaigns)
            {
                campaignsToSend.Add(campaignViewHelper.ToCampaignViewModel(campaign));
            }



            return PartialView("_CampaignPartialView", campaignsToSend);
        }



        [Authorize]
        public ActionResult MyCampaigns()
        {
            var userId = User.Identity.GetUserId();
            var userCampaigns = new List<CampaignViewHelper>();
            var myCampaigns = db.Campaigns.Where(x => x.CreatedBy == userId).ToList();
            foreach (var myCampaign in myCampaigns)
            {
                userCampaigns.Add(campaignViewHelper.ToCampaignViewModel(myCampaign));
            }

            return View(userCampaigns);
        }

        public ActionResult Search()
        {
            return View();
        }
        // GET: Campaign/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);

            if (campaign == null)
            {
                return HttpNotFound();
            }

            var campaignsToSend = campaignViewHelper.ToCampaignViewModel(campaign);

            return View(campaignsToSend);
        }

        // GET: Campaign/Create
        public ActionResult Create()
        {
            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Id");
            ViewBag.TargetAudienceId = new SelectList(db.TargetAudience, "Id", "Name");
            return View();
        }

        // POST: Campaign/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CreatedOn,CreatedBy,TargetAudienceId,CatagoryId,CampaignMessageId,IsDeleted")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Id", campaign.CampaignMessageId);

            return View(campaign);
        }

        // GET: Campaign/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;

            return View(campaign);
        }

        // POST: Campaign/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CreatedOn,CreatedBy,TargetAudienceId,CatagoryId,CampaignMessageId,IsDeleted")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Id", campaign.CampaignMessageId);

            return View(campaign);
        }

        public PartialViewResult _EditPartial_A(int? id)
        {
            ViewBag.Category = new SelectList(db.Categories, "Id", "Name");
            ViewBag.TargetAudience = new SelectList(db.TargetAudience, "Id", "Name");
            ViewBag.ContentType = new SelectList(db.ContentType, "Id", "Name");

            var campaign = db.Campaigns.Find(id);
            var campaignModel = campaignViewHelper.ToCampaignViewModel(campaign);

            return PartialView("_EditPartial_A", campaignModel);
        }

        public PartialViewResult _EditPartial_B(string content, string shortDescription, DateTime startDate, DateTime endDate, string longDescription, int? contentTypeId, int? audienceTypeId, int? categoryId, int? budget, int? Id)
        {
            var campaign = db.Campaigns.Find((int)Id);
            var campaignMsg = db.CampaignMessages.Where(x => x.Id == (int)campaign.CampaignMessageId).FirstOrDefault();
            campaignMsg.Content = content;
            campaignMsg.ShortDiscription = shortDescription;
            campaignMsg.StartDate = startDate;
            campaignMsg.EndDate = endDate;
            campaignMsg.LongDiscription = longDescription;
            campaignMsg.ContentTypeId = (int)contentTypeId;
            campaignMsg.TargetAudienceId = (int)audienceTypeId;
            campaignMsg.Budget = (int)budget;

            db.SaveChanges();

            return PartialView();
        }

        public PartialViewResult _RelatedCampaigns(int? campaignId)
        {


            return PartialView();
        }

        // GET: Campaign/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            db.Campaigns.Remove(campaign);
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
