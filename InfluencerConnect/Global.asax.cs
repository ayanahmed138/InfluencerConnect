using InfluencerConnect.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InfluencerConnect
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private void CreateRoles()
        //{
        //    var context = new ApplicationDbContext();
        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

        //    if (!roleManager.RoleExists("Admin"))
        //        roleManager.Create(new IdentityRole("Admin"));

        //    if (!roleManager.RoleExists("User"))
        //        roleManager.Create(new IdentityRole("User"));
        //}
        //private void CreateAdminUser()
        //{
        //    var context = new ApplicationDbContext();
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        //    var adminEmail = "admin@influencerconnect.com";

        //    // Check if admin already exists
        //    if (userManager.FindByEmail(adminEmail) == null)
        //    {
        //        var adminUser = new ApplicationUser
        //        {
        //            UserName = adminEmail,
        //            Email = adminEmail,
        //            EmailConfirmed = true,

        //            // 👇 Fix: Set any DateTime field with a valid value
        //            JoinedOn = DateTime.Now // 👈 Add this if your model has CreatedOn
        //        };

        //        var result = userManager.Create(adminUser, "Admin@hamdard123");

        //        if (result.Succeeded)
        //        {
        //            userManager.AddToRole(adminUser.Id, "Admin");
        //        }
        //    }
        //}


        protected void Application_Start()
        {
            //CreateRoles();
            //CreateAdminUser();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }
}
