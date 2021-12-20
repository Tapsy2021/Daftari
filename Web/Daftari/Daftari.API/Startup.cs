using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Daftari.API.Startup))]
namespace Daftari.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}