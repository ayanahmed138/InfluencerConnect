using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace InfluencerConnect.SignalR.Hubs
{
    public class MyUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            var claimsPrincipal = request.User as ClaimsPrincipal;
            return claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}