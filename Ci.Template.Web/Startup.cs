using Ci.Template.Web;

using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Ci.Template.Web
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
