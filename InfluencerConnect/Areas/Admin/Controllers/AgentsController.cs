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
    [Authorize (Roles ="Admin")]
    public class AgentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/MarketingAgents
        public ActionResult Index()
        {
            return View(db.MarketingAgents.ToList());
        }

        [HttpPost]
        public ActionResult Approve(int id)
        {
            var agent = db.MarketingAgents.Find(id);
            if (agent != null)
            {
                agent.IsApproved = true;
                db.SaveChanges();
                TempData["Success"] = "Marketing agent approved.";
            }

            return RedirectToAction("Pending");
        }

        // POST: Admin/MarketingAgents/Reject
        [HttpPost]
        public ActionResult Reject(int id)
        {
            var agent = db.MarketingAgents.Find(id);
            if (agent != null)
            {
                agent.IsApproved = false; // optional: track rejection differently
                agent.IsDeleted = true;   // or set a new `IsRejected` property
                db.SaveChanges();
                TempData["Warning"] = "Marketing agent rejected.";
            }

            return RedirectToAction("Pending");
        }

        // GET: Admin/MarketingAgents/Details/5
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

        // GET: Admin/MarketingAgents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/MarketingAgents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Name,Company,CompanyLetterName,CompanyLetterPath,ContactInfo,IsDeleted,IsApproved")] MarketingAgents marketingAgents)
        {
            if (ModelState.IsValid)
            {
                db.MarketingAgents.Add(marketingAgents);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(marketingAgents);
        }

        // GET: Admin/MarketingAgents/Edit/5
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

        // POST: Admin/MarketingAgents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Name,Company,CompanyLetterName,CompanyLetterPath,ContactInfo,IsDeleted,IsApproved")] MarketingAgents marketingAgents)
        {
            if (ModelState.IsValid)
            {
                db.Entry(marketingAgents).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(marketingAgents);
        }

        // GET: Admin/MarketingAgents/Delete/5
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

        // POST: Admin/MarketingAgents/Delete/5
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
