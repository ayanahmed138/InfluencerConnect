using InfluencerConnect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfluencerConnect.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userId = User.Identity.GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                ViewBag.UnreadCount = db.Notifications
                    .Count(n => n.UserId == userId && !n.IsRead);
            }
            else
            {
                ViewBag.UnreadCount = 0;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}