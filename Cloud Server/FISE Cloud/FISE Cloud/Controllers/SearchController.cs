using FISE_Cloud.Filters;
using FISE_Cloud.Models;
using FISE_Cloud.Models.School;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace FISE_Cloud.Controllers
{
    [FISEAuthorize(InRoles = "superadmin,elibadmin,schooladmin")]
    [NoCache]
    public class SearchController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;
        private readonly int _pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
        public SearchController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData != null ? _authService.CurrentUserData.UserId : 0);
        }

        [HttpGet]
        
        public ActionResult Search(int pageno = 1, int pagesize = 0, string query = null)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            if (query==null) {
                SearchResult model = new SearchResult();
                model.Status = GenericStatus.Other;
                return View(model);
            }
            else {
                var modelapi = _webClient.DownloadData<Search>("search", new { Role = _authService.CurrentUserData.Role, PageIndex = pageno, PageSize = pagesize, SearchTxt = query, UserId = _authService.CurrentUserData.UserId });
                SearchResult model = new SearchResult();
                model.School = new PagedList<School>(modelapi.School.Items, pageno, pagesize, modelapi.School.TotalItems);
                model.SchoolAdmin = new PagedList<UserRegistrationModel>(modelapi.SchoolAdmin.Items, pageno, pagesize, modelapi.SchoolAdmin.TotalItems);
                model.Student = new PagedList<StudentRegistrationModel>(modelapi.Student.Items, pageno, pagesize, modelapi.Student.TotalItems);
                model.Book = new PagedList<Book>(modelapi.Book.Items, pageno, pagesize, modelapi.Book.TotalItems);
                model.Status = GenericStatus.Sucess;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Search(int spageno = 1, int sapageno = 1, int stpageno = 1, int bpageno = 1, int pagesize = 0, string query = "")
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            ViewBag.search = query;
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "schoollist")
                {
                    var modelschoolapi = _webClient.DownloadData<APIPagedList<School>>("searchschoollist", new { PageIndex = spageno, PageSize = pagesize, SearchTxt = query });
                    var pmodel = new PagedList<School>(modelschoolapi.Items, spageno, pagesize, modelschoolapi.TotalItems);
                    ViewData["route"] = "search";
                    return PartialView("_SearchSchoolListPost", pmodel);
                }
                else if (target == "adminlist")
                {
                    var modeladminsapi = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("searchschooladmin", new { PageIndex = sapageno, PageSize = pagesize, SearchTxt = query });
                    var Admins = new PagedList<UserRegistrationModel>(modeladminsapi.Items, sapageno, pagesize, modeladminsapi.TotalItems);
                    return PartialView("_SearchSchoolAdminListPost", Admins);
                }
                else if (target == "studentlist")
                {
                    var modelstudentapi = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("searchstudent", new { PageIndex = stpageno, PageSize = pagesize, SearchTxt = query, UserId = _authService.CurrentUserData.UserId });
                    var Students = new PagedList<StudentRegistrationModel>(modelstudentapi.Items, stpageno, pagesize, modelstudentapi.TotalItems);
                    return PartialView("_SearchStudentListPost", Students);
                }

                else if (target == "booklist")
                {
                    var modelbookapi = _webClient.DownloadData<APIPagedList<Book>>("searchbookslist", new { PageIndex = bpageno, PageSize = pagesize, SearchTxt = query });
                    var pmodel = new PagedList<Book>(modelbookapi.Items, bpageno, pagesize, modelbookapi.TotalItems);
                    return PartialView("_SearchBooksListPost", pmodel);
                }

                var modelapi = _webClient.DownloadData<Search>("search", new { Role = _authService.CurrentUserData.Role, PageIndex = spageno, PageSize = pagesize, SearchTxt = query, UserId = _authService.CurrentUserData.UserId });
                SearchResult model = new SearchResult();
                model.School = new PagedList<School>(modelapi.School.Items, spageno, pagesize, modelapi.School.TotalItems);
                model.SchoolAdmin = new PagedList<UserRegistrationModel>(modelapi.SchoolAdmin.Items, spageno, pagesize, modelapi.SchoolAdmin.TotalItems);
                model.Student = new PagedList<StudentRegistrationModel>(modelapi.Student.Items, 1, pagesize, modelapi.Student.TotalItems);
                model.Book = new PagedList<Book>(modelapi.Book.Items, spageno, pagesize, modelapi.Book.TotalItems);
                model.Status = GenericStatus.Sucess;

                return View(model);
            }
            else
            {
                var modelapi = _webClient.DownloadData<Search>("search", new { Role = _authService.CurrentUserData.Role, PageIndex = spageno, PageSize = pagesize, SearchTxt = query, UserId = _authService.CurrentUserData.UserId });
                SearchResult model = new SearchResult();
                model.School = new PagedList<School>(modelapi.School.Items, spageno, pagesize, modelapi.School.TotalItems);
                model.SchoolAdmin = new PagedList<UserRegistrationModel>(modelapi.SchoolAdmin.Items, spageno, pagesize, modelapi.SchoolAdmin.TotalItems);
                model.Student = new PagedList<StudentRegistrationModel>(modelapi.Student.Items, spageno, pagesize, modelapi.Student.TotalItems);
                model.Book = new PagedList<Book>(modelapi.Book.Items, spageno, pagesize, modelapi.Book.TotalItems);
                model.Status = GenericStatus.Sucess;

                return View(model);
            }
        }
    }
}

