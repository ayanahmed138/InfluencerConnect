using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using InfluencerConnect.SignalR.Hubs;

[assembly: OwinStartupAttribute(typeof(InfluencerConnect.Startup))]
namespace InfluencerConnect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //Register SignalR
            app.MapSignalR();

            // Optional: Register user ID provider
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyUserIdProvider());
        }
    }
}
