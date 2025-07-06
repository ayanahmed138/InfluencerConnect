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
    [Authorize]
    public class ChatsController : BaseController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        public ChatsViewModel chatsViewModel = new ChatsViewModel();

        // GET: Chats
        public ActionResult Index(int? chatId)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = db.Users.Where(x => x.Id == currentUserId).FirstOrDefault();
            var agent = db.MarketingAgents.Where(x => x.UserId == currentUserId).FirstOrDefault();
            if(agent!=null)
            {
                if (!agent.IsApproved)
                {
                    ViewBag.BlockMessaging = true;
                }
            }

            ViewBag.IsInfluencer = user.IsInfluencer;
            ViewBag.UserImage = user.ImagePath;
            ViewBag.SelectedChatId = chatId;
            var existingChat = db.Chats.Where(c => c.User1Id == currentUserId || c.User2Id == currentUserId).ToList();
            var chatsToSend = new List<ChatsViewModel>();
            if (existingChat.Any())
            {
                foreach (var chat in existingChat)
                {
                    chatsToSend.Add(chatsViewModel.toChatViewModel(chat, currentUserId));
                }
            }

            ViewBag.ShowFooter = false;
            return View(chatsToSend);
        }

        public JsonResult GetMessages(int chatId)
        {
            var currentUserId = User.Identity.GetUserId();

            var messages = db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedOn)
                .ToList() // switch from LINQ-to-Entities to LINQ-to-Objects
                .Select(m => new {
                    content = m.Text,
                    timestamp = m.CreatedOn.ToString("dd MMM yyyy hh:mm tt"),
                    isSender = m.SenderId == currentUserId
                }).ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }


        //For StartChat button

        [HttpPost]
        public JsonResult StartChat(string targetUserId)
        {
            var currentUserId = User.Identity.GetUserId();
            var existingChat = db.Chats.FirstOrDefault(c => (c.User1Id == currentUserId && c.User2Id == targetUserId) ||
            (c.User1Id == targetUserId && c.User2Id == currentUserId) );
            if (existingChat==null)
            {
                var newChat = new Chat()
                {
                    User1Id = currentUserId,
                    User2Id = targetUserId,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,


                };
                db.Chats.Add(newChat); 
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {


            return Json(new { success = true, chatId = existingChat.Id });
            }



        }


        public ActionResult StartChatfronNotification(string targetUserId)
        {
            var currentUserId = User.Identity.GetUserId();
            var existingChat = db.Chats.FirstOrDefault(c => (c.User1Id == currentUserId && c.User2Id == targetUserId) ||
            (c.User1Id == targetUserId && c.User2Id == currentUserId));
            if (existingChat == null)
            {
                var newChat = new Chat()
                {
                    User1Id = currentUserId,
                    User2Id = targetUserId,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,


                };
                db.Chats.Add(newChat);
                db.SaveChanges();
                existingChat = newChat;
            }



            return RedirectToAction("Index", "Chats", new { chatId = existingChat.Id });




        }
        // GET: Chats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = db.Chats.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }

        // GET: Chats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Chats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsDeleted,User1Id,User2Id")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Chats.Add(chat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chat);
        }

        // GET: Chats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = db.Chats.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }

        // POST: Chats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsDeleted,User1Id,User2Id")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chat);
        }

        // GET: Chats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = db.Chats.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }

        // POST: Chats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chat chat = db.Chats.Find(id);
            db.Chats.Remove(chat);
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
