using FISE_Cloud.Filters;
using FISE_Cloud.Models;
using FISE_Cloud.Models.School;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace FISE_Cloud.Controllers
{
    [FISEAuthorize(InRoles = "schooladmin")]
    [NoCache]
    public class SchoolAdminController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;
        private readonly int _pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
        public SchoolAdminController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData != null ? _authService.CurrentUserData.UserId : 0);
        }

        [HttpGet]
        public ActionResult SchoolAdminDashboard(string schooluid, int pageno = 1, int pagesize = 0)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var user = _authService.CurrentUserData;

            var modelapi = _webClient.DownloadData<SchoolDetailsResult>("getschooladmindashboard", new { PageIndex = pageno, PageSize = pagesize, UserId = user.UserId, SchoolUId = schooluid });
            if (modelapi != null && modelapi.APIStatus != SchoolStatusEnum.NoSchoolFound)
            {
                if (user.SchoolUId != schooluid)
                    return RedirectToRoute("UnauthorizedAccess");
                SchoolDetails model = new SchoolDetails();
                model.SchoolDetail = modelapi.SchoolDetails;
                var Students = new Webdiyer.WebControls.Mvc.PagedList<StudentRegistrationModel>(modelapi.Students.Items, pageno, pagesize, modelapi.Students.TotalItems);
                model.Students = Students;
                model.Grades = modelapi.Grades;

                if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                {
                    if ((model.SchoolDetail.SchoolName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.SchoolDetail.SchoolName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.SchoolDetail.SchoolName;
                }
                return View("SchoolAdminDashboard", model);
            }
            else
                return RedirectToRoute("PageNotFound");
        }

        [HttpPost]
        public ActionResult SchoolAdminDashboard(string schooluid, string Grade="0", int pageno = 1, int pagesize = 0)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var user = _authService.CurrentUserData;
            if (user.SchoolUId != schooluid)
                return RedirectToRoute("UnauthorizedAccess");         
            var modelstudentapi = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getstudentsofschooladmin", new { PageIndex = pageno, PageSize = pagesize, SchoolUId = schooluid, Grade = Grade });
            var Students = new PagedList<StudentRegistrationModel>(modelstudentapi.Items, pageno, pagesize, modelstudentapi.TotalItems);
            return PartialView("_SchoolAdminStudentsPost", Students);
        }

        [HttpGet]
        public ActionResult StudentandParentInfoAdmin(string schooluid, string studentid)
        {
            var user = _authService.CurrentUserData;
            int _studentid;
            if(!int.TryParse(studentid,out _studentid))
                return RedirectToRoute("PageNotFound");

            var model = _webClient.DownloadData<StudentandParentInfo>("getstudentbyid", new { SchoolUId = schooluid, UserId = _studentid });
            if (model==null||model.APIStatus == 3)
                return RedirectToRoute("PageNotFound");
            if (user.SchoolUId != schooluid)
                return RedirectToRoute("UnauthorizedAccess");
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.FirstName + " " + model.LastName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ExportSchoolStudentstoExcel(string schooluid_ofstudents)
        {
            var model = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getstudentsofschool", new { PageIndex = 1, PageSize = int.MaxValue, SchoolUId = schooluid_ofstudents, Grade = "" });
            var _exportManager = new Services.ExportImport.ExportManager();
            foreach (StudentRegistrationModel student in model.Items)
            {
                if (!(student.Status) && student.IsTrashed)
                    student.Result = "Disabled";
                else if (student.Status && !(student.IsTrashed) && (student.LastLoginDate) != null)
                    student.Result = "ACTIVE";
                else if (student.Status && !(student.IsTrashed) && (student.LastLoginDate) == null)
                    student.Result = "REGISTERED";
                else if (!student.Status && !student.IsTrashed && (student.LastLoginDate) == null)
                    student.Result = "CREATED";
            }
            var bytes = _exportManager.ExportSchoolStudentsToXlsx(model.Items);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "schoolstudent.xlsx");
        }
    }
}