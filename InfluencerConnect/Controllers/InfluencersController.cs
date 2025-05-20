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
    public class InfluencersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public InfluencerListViewModel influencerListViewHelper = new InfluencerListViewModel();

        // GET: Influencers
        public ActionResult Index()
        {

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

        public PartialViewResult _SearchInfluencer()
        {
            var allinfluencers = db.Influencer.ToList();
            var influencersToSend = new List<InfluencerListViewModel>();

            foreach (var influencer in allinfluencers)
            {
                influencersToSend.Add(influencerListViewHelper.ToInfluencerListViewModel(influencer));
            }


            return PartialView(influencersToSend);
        }

        public ActionResult SearchInfluencers(List<int> categories, List<int> contentTypes, int? minPrice, int? maxPrice, string keyword)
        {
            var influencers = db.Influencer.AsQueryable();

            // Filter by category
            if (categories != null && categories.Any())
            {
                influencers = influencers.Where(i => categories.Contains(i.CategoryId));
            }

            // Filter by content types (many-to-many)
            // Filter by category
            if (contentTypes != null && contentTypes.Any())
            {
                var influencerUserIdsWithContentTypes = db.InfluencerContentType.Where(ct => contentTypes.Contains(ct.ContentTypeId))
                .Select(ct => ct.InfluencerId).Distinct()
                .ToList();

                // Then filter influencers by matching UserId
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

            // Project to viewmodel
            var influencerList = new List<InfluencerListViewModel>();

            foreach(var influencer in influencers)
            {
                influencerList.Add(influencerListViewHelper.ToInfluencerListViewModel(influencer));
            }
            

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
            if (influencer == null)
            {
                return HttpNotFound();
            }
            return View(influencer);
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
