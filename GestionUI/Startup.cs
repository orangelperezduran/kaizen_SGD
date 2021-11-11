using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GestionUI.Startup))]

namespace GestionUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
