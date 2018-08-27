using System.Web.Mvc;
using FISE_Cloud.Filters;
namespace FISE_Cloud.Controllers
{
    [NoCache]
    public class CommonController : Controller
    {
        public ActionResult UnauthorizedAccess()
        {
            return View();
        }
        public ActionResult PageNotFound()
        {
            this.Response.StatusCode = 404;
            this.Response.TrySkipIisCustomErrors = true;
            return View();
        }        
    }
}