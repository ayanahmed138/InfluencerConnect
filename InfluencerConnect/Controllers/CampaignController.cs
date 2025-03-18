using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
            var campaigns = db.Campaigns.Include(c => c.CampaignMessage);
            return View(campaigns.ToList());
        }

        [Authorize]
        public ActionResult MyCampaigns()
        {
            var userId = User.Identity.GetUserId();
            var userCampaigns = new List<CampaignViewHelper>();
            var myCampaigns = db.Campaigns.Where(x => x.CreatedBy == userId).ToList();
            foreach(var myCampaign in myCampaigns)
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
            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Id", campaign.CampaignMessageId);
          
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
