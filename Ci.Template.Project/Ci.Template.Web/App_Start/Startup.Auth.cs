namespace $safeprojectname$
{
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;

    using Owin;

    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // 讓應用程式使用 Cookie 儲存已登入使用者的資訊
            // 並使用 Cookie 暫時儲存使用者利用協力廠商登入提供者登入的相關資訊；
            // 在 Cookie 中設定簽章
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}