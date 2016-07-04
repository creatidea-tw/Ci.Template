namespace Ci.Template.Web.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// <see cref="http://materializecss.com/"/>
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class MaterializeController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult Landing()
        {
            return this.View();
        }
    }
}