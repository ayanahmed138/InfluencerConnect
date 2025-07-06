using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using InfluencerConnect.Models;
using Microsoft.AspNet.Identity;

namespace InfluencerConnect.Controllers
{
    public class NotificationsController : BaseController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        // GET: Notifications
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();

            if (currentUserId != null)
            {
                var notifications = db.Notifications
                .Where(n => n.UserId == currentUserId && n.IsDeleted == false)
                .OrderByDescending(n => n.CreatedOn)

                .ToList()
                .Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    Message = n.Message,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAgo = GetTimeAgo(n.CreatedOn)
                }).ToList();

                return View(notifications);
            }
                return View();
        }


        public PartialViewResult _NotificationPartial()
        {
            var currentUserId = User.Identity.GetUserId();

            if (currentUserId != null)
            {
                var notifications = db.Notifications
                .Where(n => n.UserId == currentUserId && n.IsDeleted==false)
                .OrderByDescending(n => n.CreatedOn)
                .Take(10)
                .ToList()
                .Select(n => new NotificationViewModel
                {
                Id = n.Id,
                Message = n.Message,
                Link = n.Link,
                IsRead = n.IsRead,
                CreatedAgo = GetTimeAgo(n.CreatedOn)
                }).ToList();
                return PartialView(notifications);
            }
            else
            {
                return PartialView();
            }
        }

        public string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";

            return dateTime.ToString("dd MMM yyyy");
        }

        [HttpPost]
        
        public JsonResult MarkAsRead(int notificationId)
        {
            var notification = db.Notifications.Find(notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Notification not found." });
        }
        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CampaignMsgId,InfluencerId,text,title,isAccepted,isDeleted")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CampaignMsgId,InfluencerId,text,title,isAccepted,isDeleted")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
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
