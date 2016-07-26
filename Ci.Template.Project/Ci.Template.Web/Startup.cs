using $safeprojectname$;

using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace $safeprojectname$
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
