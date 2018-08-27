using System;
using System.Web.Mvc;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.Filters;

namespace FISE_Cloud.Controllers
{
    [NoCache]
    public class HomeController : Controller
    {
        private readonly IAuthenticationService _authService;
        public HomeController()
        {
            this._authService = new FormsAuthenticationService();
        }

        [FISEAuthorize]
        public ActionResult Index()
        {
            var user = this._authService.CurrentUserData;
            if (user != null && user.Role.Equals("schooladmin", StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoute("SchoolAdmin", new { schooluid = user.SchoolUId });            
            return View();
        }

    }
}