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
using Microsoft.AspNet.Identity;

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

        public PartialViewResult _CreatePatial_A()
        {
           
            ViewBag.Category = new SelectList(db.Categories, "Id", "Name");
            ViewBag.TargetAudience = new SelectList(db.TargetAudience, "Id", "Name");
            ViewBag.ContentType = new SelectList(db.ContentType, "Id", "Name");

            return PartialView();
        }
        public PartialViewResult _CreatePatial_B(string content, string shortDescription, DateTime startDate, DateTime endDate, string longDescription, int? contentTypeId, int? audienceTypeId, int? categoryId)
        {
            var userId = User.Identity.GetUserId();
            var newCampaignMsg = new CampaignMessage()
            {
                Content = content,
                ShortDiscription = shortDescription,
                StartDate = startDate,
                EndDate = endDate,
                LongDiscription = longDescription,
                ContentTypeId = (int)contentTypeId,
                TargetAudienceId = (int)audienceTypeId,
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
