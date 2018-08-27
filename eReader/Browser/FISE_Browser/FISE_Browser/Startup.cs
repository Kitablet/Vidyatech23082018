using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FISE_Browser.Startup))]
namespace FISE_Browser
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
