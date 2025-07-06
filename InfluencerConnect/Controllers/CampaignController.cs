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

    public class CampaignController : BaseController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();
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

        public PartialViewResult _CampaignPartialView(int page = 1, int pageSize = 9)
        {
            var query = db.Campaigns
                .Include(c => c.CampaignMessage)
                .Where(x => x.IsDeleted == false && x.CampaignMessage.StartDate > DateTime.Now);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var campaigns = query
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var campaignsToSend = campaigns
                .Select(campaign => campaignViewHelper.ToCampaignViewModel(campaign))
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return PartialView(campaignsToSend);
        }


        public ActionResult SearchCampaigns(List<int> categories, string keyword, int page = 1, int pageSize = 9)
        {
            var query = db.Campaigns
                .Include(c => c.CampaignMessage)
                .Where(c => !c.IsDeleted && c.CampaignMessage.StartDate > DateTime.Now);

            if (categories != null && categories.Any())
                query = query.Where(c => categories.Contains(c.CatagoryId));

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(c =>
                    c.CampaignMessage.Content.ToLower().Contains(keyword) ||
                    c.CampaignMessage.ShortDiscription.ToLower().Contains(keyword) ||
                    c.CampaignMessage.LongDiscription.ToLower().Contains(keyword));
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedCampaigns = query
                .OrderByDescending(c => c.CampaignMessage.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var campaignsToSend = pagedCampaigns.Select(c => campaignViewHelper.ToCampaignViewModel(c)).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return PartialView("_CampaignPartialView", campaignsToSend);
        }



        [System.Web.Mvc.Authorize]
        public ActionResult MyCampaigns(int page = 1)
        {
            var userId = User.Identity.GetUserId();
            int pageSize = 9;

            var allMyCampaigns = db.Campaigns
                .Where(x => x.CreatedBy == userId)
                .ToList();

            var pagedCampaigns = allMyCampaigns
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userCampaigns = new List<CampaignViewHelper>();
            foreach (var myCampaign in pagedCampaigns)
            {
                userCampaigns.Add(campaignViewHelper.ToCampaignViewModel(myCampaign));
            }

            var categoryCounts = db.Categories.ToList()
                .Select(cat => new CategoryCountViewModel
                {
                    CategoryId = cat.Id,
                    CategoryName = cat.Name,
                    Count = allMyCampaigns.Count(c => c.CatagoryId == cat.Id)
                })
                .ToList();

            ViewBag.CategoryCounts = categoryCounts;

            int totalPages = (int)Math.Ceiling((double)allMyCampaigns.Count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

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
            var campaign = db.Campaigns.Where(x=>x.CampaignMessageId== campaignId).FirstOrDefault();    
            var relatedCampaigns = db.Campaigns.Where(x=>x.CatagoryId==campaign.CatagoryId && x.CampaignMessage.StartDate > DateTime.Now)
                .OrderBy(x=>x.CampaignMessage.StartDate).Take(4).ToList();

            var relatedCampaignsToSend = new List<CampaignViewHelper>();
            foreach(var cam in relatedCampaigns)
            {
                relatedCampaignsToSend.Add(campaignViewHelper.ToCampaignViewModel(cam));

            }

            return PartialView(relatedCampaignsToSend);
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

        [HttpPost]
        public JsonResult DeleteCampaign(int? id)
        {
            if(id!=null)
            {
                var campaign = db.Campaigns.Where(x => x.Id == (int)id).FirstOrDefault();
                campaign.IsDeleted = true;
                db.SaveChanges();

                return Json(new { success = true });

            }
            else
            {
                return Json(new { success = false });

            }


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
