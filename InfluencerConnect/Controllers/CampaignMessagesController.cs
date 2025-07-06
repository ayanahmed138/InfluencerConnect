using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InfluencerConnect.Models;
using InfluencerConnect.SignalR.Hubs;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace InfluencerConnect.Controllers
{
    public class CampaignMessagesController : BaseController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

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

        [System.Web.Mvc.Authorize(Roles = "Admin, User")]
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

        public PartialViewResult _CreatePatial_A(int? campaignMsgId)
        {
            var currentUserId = User.Identity.GetUserId();
            var agent = db.MarketingAgents.Where(x => x.UserId == currentUserId).FirstOrDefault();

            if(agent!=null)
            {
                ViewBag.IsApproved = agent.IsApproved;
            }

            if (campaignMsgId != null)
            {
                var campaign = db.Campaigns.Where(x => x.CampaignMessageId == (int)campaignMsgId).FirstOrDefault();

                ViewBag.Category = new SelectList(db.Categories, "Id", "Name", campaign.CatagoryId);
                ViewBag.TargetAudience = new SelectList(db.TargetAudience, "Id", "Name", campaign.CampaignMessage.TargetAudienceId);
                ViewBag.ContentType = new SelectList(db.ContentType, "Id", "Name", campaign.CampaignMessage.ContentTypeId);

                return PartialView(db.CampaignMessages.Where(x => x.Id == (int)campaignMsgId).FirstOrDefault());
            }
            else
            {
                ViewBag.Category = new SelectList(db.Categories, "Id", "Name");
                ViewBag.TargetAudience = new SelectList(db.TargetAudience, "Id", "Name");
                ViewBag.ContentType = new SelectList(db.ContentType, "Id", "Name");
            }


            return PartialView();
        }
        public PartialViewResult _CreatePatial_B(string content, string shortDescription, DateTime startDate, DateTime endDate, string longDescription, int? contentTypeId, int? audienceTypeId, int? categoryId, int? budget, int? campaignMsgId)
        {
            var userId = User.Identity.GetUserId();
            if (campaignMsgId != null)
            {
                var campaignMsg = db.CampaignMessages.Where(x => x.Id == (int)campaignMsgId).FirstOrDefault();
                campaignMsg.Content = content;
                campaignMsg.ShortDiscription = shortDescription;
                campaignMsg.StartDate = startDate;
                campaignMsg.EndDate = endDate;
                campaignMsg.LongDiscription = longDescription;
                campaignMsg.ContentTypeId = (int)contentTypeId;
                campaignMsg.TargetAudienceId = (int)audienceTypeId;
                campaignMsg.Budget = (int)budget;
                var campaign = db.Campaigns.Where(x => x.CampaignMessageId == (int)campaignMsgId).FirstOrDefault();
                campaign.CatagoryId = (int)categoryId;
                db.SaveChanges();
                var campaignImages = db.CampaignImages.Where(x => x.CampaignMsgId == (int)campaignMsgId).ToList();
                ViewBag.CampaignMsgId = campaignMsg.Id;
                return PartialView(campaignImages);

            }
            else
            {
                var newCampaignMsg = new CampaignMessage()
                {
                    Content = content,
                    ShortDiscription = shortDescription,
                    StartDate = startDate,
                    EndDate = endDate,
                    LongDiscription = longDescription,
                    ContentTypeId = (int)contentTypeId,
                    TargetAudienceId = (int)audienceTypeId,
                    Budget = (int)budget,
                    IsDeleted = false,
                };

                db.CampaignMessages.Add(newCampaignMsg);
                db.SaveChanges();
                var newCampaign = new Campaign()
                {
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    CatagoryId = (int)categoryId,
                    IsPrivate = false,
                    IsDeleted = false,
                    CampaignMessageId = newCampaignMsg.Id,
                };

                ViewBag.CampaignMsgId = newCampaignMsg.Id;

                db.Campaigns.Add(newCampaign);
                db.SaveChanges();
            }

            return PartialView();
        }

        [HttpPost]
        public JsonResult UploadImages(int campaignMsgId)
        {
            try
            {
                var uploadPath = Server.MapPath("~/Content/uploads/"); // Save inside Content/uploads
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var existingImages = db.CampaignImages.Where(img => img.CampaignMsgId == campaignMsgId).ToList();

                foreach (var image in existingImages)
                {
                    var fullPath = Server.MapPath(image.FilePath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath); // Delete from disk
                    }

                    db.CampaignImages.Remove(image); // Delete from DB
                }

                db.SaveChanges(); // Save the deletion changes first
                List<object> savedFiles = new List<object>();

                foreach (string fileKey in Request.Files)
                {
                    var file = Request.Files[fileKey];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(uploadPath, fileName);

                        file.SaveAs(filePath); // Save image

                        // Save details to DB
                        var imageRecord = new CampaignImages
                        {
                            CampaignMsgId = campaignMsgId, // Save campaignMsgId
                            FileName = fileName,
                            FilePath = "/Content/uploads/" + fileName,
                            IsDeleted = false
                        };

                        db.CampaignImages.Add(imageRecord);
                        db.SaveChanges();

                        savedFiles.Add(new { fileName, filePath = imageRecord.FilePath });
                    }
                }

                return Json(new { success = true, files = savedFiles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        
        public ActionResult InfluencerSearch(int? campaignMsgId)
        {
            var campaign = db.Campaigns.Where(x => x.CampaignMessageId == (int)campaignMsgId).FirstOrDefault();


            return View("Index", "Influencers");
        }

        public ActionResult GoToCampaignDetailsFromMsg(int? campaignMsgId)
        {
            var campaign = db.Campaigns.FirstOrDefault(c => c.CampaignMessageId == campaignMsgId);
            if (campaign != null)
            {
                return RedirectToAction("Details", "Campaign", new { id = campaign.Id });
            }

            return HttpNotFound(); // or redirect to error page

        }

        
        [HttpPost]
        public JsonResult InviteInfluencers(int? campaignMsgId, List<string> InfluencerIds)
        {
            if (campaignMsgId != null && InfluencerIds.Count > 0)
            {
                var currentUserId = User.Identity.GetUserId();
                var failedInvites = new List<string>();

                foreach (string influencerUserId in InfluencerIds)
                {
                    // Get influencer from DB
                    var influencer = db.Influencer.FirstOrDefault(i => i.UserId == influencerUserId);
                    if (influencer == null) continue;

                    // Count existing non-deleted invitations
                    int currentInviteCount = db.Invitation.Count(inv =>
                        inv.InfluencerId == influencerUserId && !inv.IsDeleted);

                    // Check limit
                    if (currentInviteCount >= influencer.Limit)
                    {
                        failedInvites.Add(influencer.Name ?? influencerUserId);
                        continue;
                    }

                    // Optional: skip duplicate invites for same campaign
                    bool alreadyInvited = db.Invitation.Any(x =>
                        x.CampaignMsgId == campaignMsgId && x.InfluencerId == influencerUserId && !x.IsDeleted);

                    if (alreadyInvited) continue;

                    // Create new invitation
                    var newInvitation = new Invitation
                    {
                        CampaignMsgId = (int)campaignMsgId,
                        InfluencerId = influencerUserId,
                        AgentId = currentUserId,
                        IsAccepted = false,
                        IsDeleted = false,
                        CreateOn = DateTime.Now
                    };

                    db.Invitation.Add(newInvitation);

                    var newNotification = new Notification()
                    {
                        Link = "/CampaignMessages/GoToCampaignDetailsFromMsg?campaignMsgId=" + (int)campaignMsgId,
                        Message = "New campaign invitation. Click to view",
                        CreatedOn = DateTime.Now,
                        UserId = influencerUserId,
                        IsDeleted = false,
                        IsRead = false,
                    };

                    db.Notifications.Add(newNotification);

                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.User(influencerUserId)
                        .ReceiveNotification("New campaign invitation. Click to view", "/CampaignMessages/GoToCampaignDetailsFromMsg?campaignMsgId=" + (int)campaignMsgId);

                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    failed = failedInvites.Count > 0 ? failedInvites : null
                });
            }
            else
            {
                return Json(new { success = false });
            }
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
            ViewBag.CampaignMsgId = campaignMessage.Id;

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
