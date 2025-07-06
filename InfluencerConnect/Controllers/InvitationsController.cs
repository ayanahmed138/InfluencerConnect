using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InfluencerConnect.Models;
using InfluencerConnect.SignalR.Hubs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace InfluencerConnect.Controllers
{
    public class InvitationsController : BaseController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invitations
        public ActionResult Index()
        {
            var invitation = db.Invitation.Include(i => i.CampaignMessage);
            return View(invitation.ToList());
        }

        [HttpPost]
        public JsonResult HandleAction(int? id, string action)
        {
            var invite = db.Invitation.Where(x => x.Id == id).FirstOrDefault();

            var currentuserId = User.Identity.GetUserId();
            if (invite == null || invite.IsDeleted == true)
            {
                return Json(new { success = false, action = action });
            }
           if(action=="accept")
            {
                invite.IsAccepted = true;

                var newNotification = new Notification()
                {
                    UserId = invite.AgentId,
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                    IsRead = false,
                    Link = "/Chats/StartChatfronNotification?targetUserId=" + currentuserId,
                    Message = "Invitation Accpeted for" + invite.CampaignMessage.Content + " Click to Chat",

                };

                db.Notifications.Add(newNotification);
                db.SaveChanges();

                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.User(invite.AgentId)
                    .ReceiveNotification("Invitation Accpeted for" + invite.CampaignMessage.Content + " Click to Chat", "/Chats/StartChatfronNotification?targetUserId=" + currentuserId);
               
                return Json(new { success = true, action = action });

            }
           else if(action=="reject")
            {
                invite.IsDeleted = true;
                var newNotification = new Notification()
                {
                    UserId = invite.AgentId,
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                    IsRead = false,
                    Link = "#",
                    Message = "Invitation Declined for" + invite.CampaignMessage.Content,

                };

                db.SaveChanges();

                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.User(invite.AgentId)
                    .ReceiveNotification("Invitation Declined for" + invite.CampaignMessage.Content, "#");

                return Json(new { success = true, action = action });
            }
           else
            {
                return Json(new { success = false, action = action });
            }
        }

        // GET: Invitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitation.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // GET: Invitations/Create
        public ActionResult Create()
        {
            ViewBag.CampaignMsgId = new SelectList(db.CampaignMessages, "Id", "Content");
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CampaignMsgId,InfluencerId,AgentId,CreateOn,IsAccepted,IsDeleted")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Invitation.Add(invitation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CampaignMsgId = new SelectList(db.CampaignMessages, "Id", "Content", invitation.CampaignMsgId);
            return View(invitation);
        }

        // GET: Invitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitation.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            ViewBag.CampaignMsgId = new SelectList(db.CampaignMessages, "Id", "Content", invitation.CampaignMsgId);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CampaignMsgId,InfluencerId,AgentId,CreateOn,IsAccepted,IsDeleted")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CampaignMsgId = new SelectList(db.CampaignMessages, "Id", "Content", invitation.CampaignMsgId);
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitation.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitation.Find(id);
            db.Invitation.Remove(invitation);
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
