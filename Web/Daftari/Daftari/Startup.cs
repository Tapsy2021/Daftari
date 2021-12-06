using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Daftari.Startup))]
namespace Daftari
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
