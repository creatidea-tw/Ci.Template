namespace $safeprojectname$
{
    using System.Security.Claims;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Ci.Mms.Admin;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // 設定AntiForgeryToken依據
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}
