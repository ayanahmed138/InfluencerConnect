using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InfluencerConnect.Models;

namespace InfluencerConnect.Areas.Admin.Controllers
{
    public class CampaignManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/CampaignManager
        public ActionResult Index()
        {
            var campaigns = db.Campaigns.Include(c => c.CampaignMessage);
            return View(campaigns.ToList());
        }

        // GET: Admin/CampaignManager/Details/5
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
            return View(campaign);
        }

        // GET: Admin/CampaignManager/Create
        public ActionResult Create()
        {
            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Content");
            return View();
        }

        // POST: Admin/CampaignManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CreatedOn,CreatedBy,CatagoryId,CampaignMessageId,IsPrivate,IsDeleted")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Content", campaign.CampaignMessageId);
            return View(campaign);
        }

        // GET: Admin/CampaignManager/Edit/5
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
            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Content", campaign.CampaignMessageId);
            return View(campaign);
        }

        // POST: Admin/CampaignManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CreatedOn,CreatedBy,CatagoryId,CampaignMessageId,IsPrivate,IsDeleted")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CampaignMessageId = new SelectList(db.CampaignMessages, "Id", "Content", campaign.CampaignMessageId);
            return View(campaign);
        }

        // GET: Admin/CampaignManager/Delete/5
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

        // POST: Admin/CampaignManager/Delete/5
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
