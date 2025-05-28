using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InfluencerConnect.Startup))]
namespace InfluencerConnect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
