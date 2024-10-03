using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InfluencerConnect.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinedOn { get; set; }
        public bool IsInfluencer { get; set; }   
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {


            return new ApplicationDbContext();

        }
        public DbSet<Category> Categories { get; set; }
       // public DbSet<Chat> Chats { get; set; }
       // public DbSet<Message> Messages { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignMessage> CampaignMessages { get; set; }
        public DbSet<Influencer> Influencer { get; set; }
        public DbSet<MarketingAgents> MarketingAgents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<InfluencerContentType> InfluencerContentType { get; set; }
        public DbSet<TargetAudience> TargetAudience { get; set; }
        //public DbSet<Transactions> Transactions { get; set; }
        public DbSet<ContentType> ContentType { get; set; }


        
        
    }
}