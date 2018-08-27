using System.Web.Mvc;

namespace FISE_Browser.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error()
        {
            return View("CommonError");
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult UnauthorizedForbidden()
        {
            return View();
        }
    }
}