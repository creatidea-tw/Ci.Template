namespace Ci.Template.Service.Helpers
{
    using System.Web;

    public class NetHelper
    {
        /// <summary>
        /// 取得IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }
    }
}
