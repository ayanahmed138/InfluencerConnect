﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfluencerConnect.Areas.Admin.Controllers
{
    [Authorize (Roles ="Admin")]
    public class AdminDashBoardController : Controller
    {
        // GET: Admin/AdminDashBoard
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Agents()
        {

            return View();
        }
    }
}