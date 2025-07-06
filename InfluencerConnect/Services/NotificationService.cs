using InfluencerConnect.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using InfluencerConnect.Models;


namespace InfluencerConnect.Services
{
    public static class NotificationService
    {
        
        public static void NotifyUser(string userId, string message, string link)
        {
            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.User(userId).ReceiveNotification(message, link);
                Debug.WriteLine($"✅ Notification sent to {userId}");

                using (var db = new ApplicationDbContext())
                {
                    var newNotification = new Notification()
                    {
                        Link = link,
                        Message = message,
                        CreatedOn = DateTime.Now,
                        UserId = userId,
                        IsDeleted = false,
                        IsRead = false,
                    };

                    db.Notifications.Add(newNotification);
                    db.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Notification failed: " + ex.Message);
            }
        }
    }
}