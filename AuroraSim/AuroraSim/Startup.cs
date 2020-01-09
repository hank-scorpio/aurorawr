using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AuroraSim.Startup))]

namespace AuroraSim
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
