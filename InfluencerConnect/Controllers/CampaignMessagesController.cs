using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InfluencerConnect.Models;

namespace InfluencerConnect.Controllers
{
    public class CampaignMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CampaignMessages
        public ActionResult Index()
        {
            return View(db.CampaignMessages.ToList());
        }

        // GET: CampaignMessages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignMessage campaignMessage = db.CampaignMessages.Find(id);
            if (campaignMessage == null)
            {
                return HttpNotFound();
            }
            return View(campaignMessage);
        }

        // GET: CampaignMessages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CampaignMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsDeleted")] CampaignMessage campaignMessage)
        {
            if (ModelState.IsValid)
            {
                db.CampaignMessages.Add(campaignMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campaignMessage);
        }

        public ActionResult Create2()
        {
            return View();
        }

        // GET: CampaignMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignMessage campaignMessage = db.CampaignMessages.Find(id);
            if (campaignMessage == null)
            {
                return HttpNotFound();
            }
            return View(campaignMessage);
        }

        // POST: CampaignMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsDeleted")] CampaignMessage campaignMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaignMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(campaignMessage);
        }

        // GET: CampaignMessages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignMessage campaignMessage = db.CampaignMessages.Find(id);
            if (campaignMessage == null)
            {
                return HttpNotFound();
            }
            return View(campaignMessage);
        }

        // POST: CampaignMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CampaignMessage campaignMessage = db.CampaignMessages.Find(id);
            db.CampaignMessages.Remove(campaignMessage);
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
