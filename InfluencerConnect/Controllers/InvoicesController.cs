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
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNet.Identity;
using InfluencerConnect.SignalR.Hubs;
using Microsoft.AspNet.SignalR;

namespace InfluencerConnect.Controllers
{
    public class InvoicesController : BaseController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invoices
        public ActionResult Index()
        {
            return View(db.Invoices.ToList());
        }

        [HttpPost]
        
        public JsonResult GenerateAndSend(Invoice model)
        {
            var senderId = User.Identity.GetUserId();
            var chat = db.Chats.Find(model.ChatId);
            if (chat == null) return Json(new { success = false });

            var receiverId = (chat.User1Id == senderId) ? chat.User2Id : chat.User1Id;

            model.SenderId = senderId;
            model.ReceiverId = receiverId;
            model.CreatedOn = DateTime.Now;

            // Save invoice first
            db.Invoices.Add(model);
            db.SaveChanges();

            // Generate PDF
            var pdfPath = GeneratePdf(model);
            model.PdfPath = pdfPath;
            db.SaveChanges();

            
            // Send message in chat (optional)
            var newMessage = new Messages
            {
                ChatId = model.ChatId,
                CreatedOn = DateTime.Now,
                SenderId = senderId,
                Text = $"Invoice generated. <a href='{pdfPath}' target='_blank'>Click to view</a>",
                IsViewed = false,
                IsDeleted = false
            };


            db.Messages.Add(newMessage);
            db.SaveChanges();

            string messageHtml = $"Invoice generated. <a href='{pdfPath}' target='_blank'>Click to view</a>";
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            
            
            context.Clients.User(receiverId).ReceiveMessage(
                senderId,
                messageHtml,  // HTML with invoice link
                DateTime.Now.ToString("dd MMM yyyy hh:mm tt"),
                false,
                model.ChatId
            );

            


            return Json(new {
                success = true,
                messageHtml = messageHtml,
                timestamp = DateTime.Now.ToString("dd MMM yyyy hh:mm tt")
            });
        }

        public string GeneratePdf(Invoice invoice)
        {
            var fileName = $"Invoice_{invoice.Id}.pdf";
            var folderPath = Server.MapPath("~/Content/Invoices");
            var fullPath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                var doc = new Document(PageSize.A4, 50, 50, 80, 50);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                // Add logo if available
                string logoPath = Server.MapPath("~/Content/Images/IClogo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    var logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.ScaleToFit(100f, 100f);
                    logo.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(logo);
                }

                // Fonts
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                // Invoice Title
                doc.Add(new Paragraph("Invoice", titleFont) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph("\n"));

                // Fields
                doc.Add(new Paragraph("Amount:", labelFont));
                doc.Add(new Paragraph(invoice.Amount.ToString(), valueFont));
                doc.Add(new Paragraph("Bank:", labelFont));
                doc.Add(new Paragraph(invoice.BankName, valueFont));
                doc.Add(new Paragraph("Account Title:", labelFont));
                doc.Add(new Paragraph(invoice.BankAccountTitle, valueFont));
                doc.Add(new Paragraph("Account Number:", labelFont));
                doc.Add(new Paragraph(invoice.BankAccountNumber, valueFont));
                doc.Add(new Paragraph("Description:", labelFont));
                doc.Add(new Paragraph(invoice.Description, valueFont));

                // Line separator and footer
                doc.Add(new Paragraph("\n"));
                doc.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2)));
                doc.Add(new Paragraph("Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt"), FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10)));

                doc.Close();
            }

            return "/Content/Invoices/" + fileName;
        }




        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SenderId,ReceiverId,ChatId,BankAccountTitle,BankAccountNumber,BankName,Amount,Description,PdfPath,CreatedOn,IsDeleted")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Invoices.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SenderId,ReceiverId,ChatId,BankAccountTitle,BankAccountNumber,BankName,Amount,Description,PdfPath,CreatedOn,IsDeleted")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
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
