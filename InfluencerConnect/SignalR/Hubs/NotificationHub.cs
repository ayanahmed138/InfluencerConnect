using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace InfluencerConnect.SignalR.Hubs
{
    public class NotificationHub : Hub
    {


        public override Task OnConnected()
        {
            var userId = (Context.User as ClaimsPrincipal)?
                         .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Optional: confirm connection
            Clients.Caller.ShowUserId(userId);

            return base.OnConnected();
        }
    }
}