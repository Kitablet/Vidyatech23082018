using FISE_Cloud.Filters;
using FISE_Cloud.Models;
using FISE_Cloud.Models.School;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients;
using FISE_Cloud.Validators.User;
using System.Linq;
using System.Web.Mvc;

namespace FISE_Cloud.Controllers
{
    [NoCache]
    /// <summary>
    /// This controller will handle parent related requests
    /// </summary>
    public class ParentController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;

        public ParentController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData != null ? _authService.CurrentUserData.UserId : 0);
        }

        /// <summary>
        /// This will get the required data from API to render the dashboard of a parent.
        /// </summary>
        [ChildActionOnly]
        [FISEAuthorize(InRoles = "parent")]
        public ActionResult ParentDashboard(int pageindex, int pagesize = 2)
        {
            var apimodel = _webClient.DownloadData<ParentDashboardModel>("getparentdashbord", new { UserId = _authService.CurrentUserData.UserId });
            ParentDashboardResult model = new ParentDashboardResult();
            model.Status = apimodel.Status;
            model.ActivationPending = apimodel.Students.Where(s => string.IsNullOrWhiteSpace(s.RegistrationDate.ToString())).ToList();
            model.Active = apimodel.Students.Where(s => !string.IsNullOrWhiteSpace(s.RegistrationDate.ToString())).ToList();
            return PartialView("_ParentDashboard", model);
        }

        public ActionResult Parent_Home()
        {
            return View();
        }

        /// <summary>
        /// This will render the report list page for a parent.
        /// </summary>
        [HttpGet]
        public ActionResult Report1()
        {
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report1");
        }

        #region Student Registeration
        /// <summary>
        /// Get the details of a student to render the registeration form
        /// </summary>
        [FISEAuthorize(InRoles = "parent")]
        [HttpGet]
        public ActionResult StudentRegistration(string studentid, string schooluid)
        {
            int _studentid;
            if(!int.TryParse(studentid,out _studentid))
                return RedirectToRoute("PageNotFound");
            var model = _webClient.DownloadData<StudentProfile>("getstudentprofile", new { UserId = _studentid, SchoolUId = schooluid });
            if (model == null || model.APIStatus == 3)
                return RedirectToRoute("PageNotFound");
            if (model.DateOfBirth != null)
            {
                model.DobDate = model.DateOfBirth.Value.Day;
                model.DobMonth = model.DateOfBirth.Value.Month;
                model.DobYear = model.DateOfBirth.Value.Year;
            }
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if (model.RegistrationDate==null)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName + "  >  Activate";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName;
            }
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToRoute("HomePage");
            }
        }
        /// <summary>
        /// Register a student
        /// </summary>
        [FISEAuthorize(InRoles = "parent")]
        [HttpPost]
        public ActionResult StudentRegistration(StudentProfile model)
        {
            FluentValidation.IValidator<StudentRegistrationModel> validator = new StudentRegistrationValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                var result = _webClient.UploadData<int>("registerstudent", model);
                switch (result)
                {
                    case 1:
                        return View("~/Views/User/RegisterResult.cshtml");

                    case 2:
                        ModelState.AddModelError("", Resource.StudentCreation_StudentAlreadyExists);
                        break;
                    case 3:
                        ModelState.AddModelError("", Resource.InvalidToken);
                        break;
                    default:
                        ModelState.AddModelError("", Resource.StudentCreation_unknownerror);
                        break;
                }

            }
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if (!model.Status)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName + "  >  Activate";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName;
            }
            return View(model);
        }
        #endregion
    }
}