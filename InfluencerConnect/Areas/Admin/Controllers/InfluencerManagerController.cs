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
    [Authorize(Roles ="Admin")]
    public class InfluencerManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/InfluencerManager
        public ActionResult Index()
        {
            var influencer = db.Influencer.Include(i => i.Category);
            return View(influencer.ToList());
        }

        // GET: Admin/InfluencerManager/Details/5
        public ActionResult Details(int? id)
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

        // GET: Admin/InfluencerManager/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/InfluencerManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Name,ContactInfo,MinCharge,MaxCharge,Limit,IsDeleted,CategoryId,YoutubeLink,TikTokLink,InstagramLink,AboutMe")] Influencer influencer)
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

        // GET: Admin/InfluencerManager/Edit/5
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

        // POST: Admin/InfluencerManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Name,ContactInfo,MinCharge,MaxCharge,Limit,IsDeleted,CategoryId,YoutubeLink,TikTokLink,InstagramLink,AboutMe")] Influencer influencer)
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

        // GET: Admin/InfluencerManager/Delete/5
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

        // POST: Admin/InfluencerManager/Delete/5
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
