using InfluencerConnect.Models;
using InfluencerConnect.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;

namespace InfluencerConnect.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // Maps userId to currently viewed chatId
        private static Dictionary<string, int> userActiveChats = new Dictionary<string, int>();

        public Task UpdateActiveChat(int chatId)
        {
            var userId = Context.User.Identity.GetUserId();
            lock (userActiveChats)
            {
                userActiveChats[userId] = chatId;
            }
            return Task.CompletedTask;
        }
        public async Task SendMessage(int chatId, string receiverUserId, string message)
        {
            var senderUserId = Context.User.Identity.GetUserId();
            var timestamp = DateTime.Now;

            // Save message in database with chatId, senderUserId, message, timestamp etc.
            SaveMessageToDatabase(chatId, senderUserId, message);

            // Send to receiver
            await Clients.User(receiverUserId).ReceiveMessage(senderUserId, message, timestamp.ToString("dd MMM yyyy hh:mm tt"), false, chatId);

            // Send back to sender
            await Clients.Caller.ReceiveMessage(senderUserId, message, timestamp.ToString("dd MMM yyyy hh:mm tt"), true, chatId);

            if (!userActiveChats.TryGetValue(receiverUserId, out int activeChatId) || activeChatId != chatId)
            {

                NotificationService.NotifyUser(receiverUserId, "New Message from Agent", "/Chats/Index?chatId=" + chatId);


            }
        }

        

        public override System.Threading.Tasks.Task OnConnected()
        {
            var userId = (Context.User as ClaimsPrincipal)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Clients.Caller.ShowUserId(userId);

            return base.OnConnected();
        }


        public void SaveMessageToDatabase(int chatId, string senderUserId, string message)
        {
            var newMessage = new Messages()
            {
                ChatId = chatId,
                CreatedOn = DateTime.Now,
                SenderId = senderUserId,
                IsDeleted = false,
                IsViewed = false,
                Text = message,

            };

            db.Messages.Add(newMessage);
            db.SaveChanges();
            
        }

       
        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = Context.User.Identity.GetUserId();
            lock (userActiveChats)
            {
                userActiveChats.Remove(userId);
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}