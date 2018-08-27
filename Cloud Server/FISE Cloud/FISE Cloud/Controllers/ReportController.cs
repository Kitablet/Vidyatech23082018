using FISE_Cloud.Filters;
using FISE_Cloud.Models;
using FISE_Cloud.Models.School;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;


namespace FISE_Cloud.Controllers
{
    [FISEAuthorize(InRoles = "superadmin,elibadmin,schooladmin,parent")]
    [NoCache]
    public class ReportController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;
        private readonly int _pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
        public ReportController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData != null ? _authService.CurrentUserData.UserId : 0);
        }

        /// <summary>
        /// Report 1: REGISTRATIONS AND LOGINS : Get list of avilable school to filter report
        /// </summary>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]
        public ActionResult Report1()
        {
            var model = _webClient.DownloadData<List<School>>("getschools", new { UserId = _authService.CurrentUserData.UserId });
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report1", model);
        }

        /// <summary>
        /// Report 1: REGISTRATIONS AND LOGINS :Get data for report
        /// </summary>
        [HttpPost]
        public ActionResult Report1(List<int> SchoolIds, List<string> UserTypes, int cpageno = 1, int rpageno = 1, int apageno = 1, int pagesize = 0)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }

            var target = Request.QueryString["target"];
            if (target == "created")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getregistrationandloginreportlist", new { UserType = UserTypes != null ? string.Join(",", UserTypes) : "", SchoolIds = SchoolIds != null ? string.Join(",", SchoolIds) : "", PageSize = pagesize, PageIndex = cpageno, Type = "created" });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, cpageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1ListPost", model);
            }
            else if (target == "registered")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getregistrationandloginreportlist", new { UserType = UserTypes != null ? string.Join(",", UserTypes) : "", SchoolIds = SchoolIds != null ? string.Join(",", SchoolIds) : "", PageSize = pagesize, PageIndex = rpageno, Type = "registered" });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, rpageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1ListPost2", model);
            }
            if (target == "active")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getregistrationandloginreportlist", new { UserType = UserTypes != null ? string.Join(",", UserTypes) : "", SchoolIds = SchoolIds != null ? string.Join(",", SchoolIds) : "", PageSize = pagesize, PageIndex = apageno, Type = "active" });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, apageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1ListPost3", model);
            }
            else
            {
                var apimodel = _webClient.DownloadData<RegistrationAndLoginResult>("getregistrationandloginreport", new { UserType = UserTypes != null ? string.Join(",", UserTypes) : "", SchoolIds = SchoolIds != null ? string.Join(",", SchoolIds) : "", PageSize = pagesize, PageIndex = cpageno });
                RegistrationAndLogin model = new RegistrationAndLogin();
                model.Created = new PagedList<StudentRegistrationModel>(apimodel.Created.Items, cpageno, pagesize, apimodel.Created.TotalItems);
                model.Registered = new PagedList<StudentRegistrationModel>(apimodel.Registered.Items, cpageno, pagesize, apimodel.Registered.TotalItems);
                model.Active = new PagedList<StudentRegistrationModel>(apimodel.Active.Items, cpageno, pagesize, apimodel.Active.TotalItems);
                ViewBag.Page = "cpageno";
                ViewBag.Type = "created";
                ViewBag.Container = "CreatedContainer";
                return PartialView("~/Views/Report/_Report1.cshtml", model);
            }
        }

        [HttpGet]
        public ActionResult ExportReport1()
        {
            return RedirectToRoute("Report1");
        }
        /// <summary>
        ///Report 1: REGISTRATIONS AND LOGINS : Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport1(string SchoolIdsExport = "", string UserTypesExport = "", string type = "all")
        {
            List<StudentRegistrationModel> model = new List<StudentRegistrationModel>();
            if (type == "all")
            {
                var apimodel = _webClient.DownloadData<RegistrationAndLoginResult>("getregistrationandloginreport", new { UserType = UserTypesExport, SchoolIds = SchoolIdsExport, PageSize = int.MaxValue, PageIndex = 1 });
                model.InsertRange(0, apimodel.Created.Items);
                model.InsertRange(0, apimodel.Registered.Items);
                model.InsertRange(0, apimodel.Active.Items);
            }
            else
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getregistrationandloginreportlist", new { UserType = UserTypesExport, SchoolIds = SchoolIdsExport, PageSize = int.MaxValue, PageIndex = 1, Type = type });
                model = apimodel.Items;
            }           
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport1ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report1.xlsx");
        }

        /// <summary>
        /// Report 3: Book wise usage of kitablet by students : Get data for filter
        /// </summary>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin,schooladmin")]
        public ActionResult Report3()
        {
            var model = _webClient.DownloadData<Report3FilterModel>("getreport3fillters", new { UserId = _authService.CurrentUserData.UserId });
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report3", model);
        }

        /// <summary>
        ///  Report 3: Book wise usage of kitablet by students :Get data for report
        /// </summary>
        [HttpPost]
        public ActionResult Report3(List<string> subsection, List<string> language, string duration, string city, string month, string year, List<string> book)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (duration == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)), 23, 59, 59);
            }
            else if (duration == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month) + 2, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month) + 2),23,59,59);
            }
            else if (duration == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(year), 1, 1);
                enddate = new DateTime(Convert.ToInt32(year), 12, 31,23,59,59);
            }
            else if (duration == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);;
            }
            var model = _webClient.DownloadData<Report3>("getreport3", new { StartDate = startdate, EndDate = enddate, LanguageIds = language != null ? string.Join(",", language) : "", SubsectionIds = subsection != null ? string.Join(",", subsection) : "", BookIds = book != null ? string.Join(",", book) : "", City = city });

            return PartialView("_Report3", model);
        }

        /// <summary>
        /// Report 4: Book wise usage of kitablet by students for Publishers : Get data for filter
        /// </summary>
        /// 
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]
        public ActionResult Report4()
        {
            var model = _webClient.DownloadData<Report4FilterModel>("getreport4fillters", null);
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report4", model);
        }
        /// <summary>
        ///  Report 4: Book wise usage of kitablet by students for Publishers :Get data for report
        /// </summary>      
        [HttpPost]
        public ActionResult Report4(List<string> subsection, List<string> language, string duration, string city, string month, string year, List<string> publisher)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (duration == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)), 23, 59, 59);
            }
            else if (duration == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month) + 2, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month) + 2), 23, 59, 59);
            }
            else if (duration == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(year), 1, 1);
                enddate = new DateTime(Convert.ToInt32(year), 12, 31, 23, 59, 59);
            }
            else if (duration == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            }
            var model = _webClient.DownloadData<Report4>("getreport4", new { StartDate = startdate, EndDate = enddate, LanguageIds = language != null ? string.Join(",", language) : "", SubsectionIds = subsection != null ? string.Join(",", subsection) : "", Publisher = publisher != null ? string.Join(",", publisher) : "", City = city });

            return PartialView("_Report4", model);
        }

        /// <summary>
        /// Report 5: Individual School vs. All School Average : Get data for filter
        /// </summary>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin,schooladmin")]
        public ActionResult Report5()
        {
            var model = _webClient.DownloadData<Report5FilterModel>("getreport5fillters", new { UserId = _authService.CurrentUserData.UserId });
            var user = this._authService.CurrentUserData;
            
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if (user.Role.Equals("schooladmin", StringComparison.CurrentCultureIgnoreCase))
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (Resource.Reports_Schooladmin_Report4_Title).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            }
            return View("Report5", model);
        }
        /// <summary>
        ///  Report 5: Individual School vs. All School Average :Get data for report
        /// </summary>
        [HttpPost]
        public ActionResult Report5(List<string> grade, string duration, string city, string month, string year, int school)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (duration == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)), 23, 59, 59);
            }
            else if (duration == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month) + 2, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month) + 2), 23, 59, 59);
            }
            else if (duration == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(year), 1, 1);
                enddate = new DateTime(Convert.ToInt32(year), 12, 31, 23, 59, 59);
            }
            else if (duration == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            }
            var model = _webClient.DownloadData<Report5>("getreport5", new { StartDate = startdate, EndDate = enddate, GradeIds = grade != null ? string.Join(",", grade) : "", SchoolId = school });

            return PartialView("_Report5", model);
        }

        [HttpGet]
        public ActionResult ExportReport5()
        {
            return RedirectToRoute("Report5");
        }
        /// <summary>
        ///Report 5: Individual School vs. All School Average: Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport5(string gradeexport, string durationexport, string cityexport, string monthexport, string yearexport, int schoolexport)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (durationexport == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport)));
            }
            else if (durationexport == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2, DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2));
            }
            else if (durationexport == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), 1, 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), 12, 31);
            }
            else if (durationexport == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = DateTime.Now;
            }
            var model = _webClient.DownloadData<List<Report5Export>>("getreport5export", new { StartDate = startdate, EndDate = enddate, GradeIds = gradeexport, SchoolId = schoolexport });

            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport5ToXlsx(model.Skip(0).Take(1), model.Skip(1).Take(1));
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report5.xlsx");
        }

        /// <summary>
        /// Report 6: Active Books in Kitablet : Get data for filter
        /// </summary>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin,schooladmin")]
        public ActionResult Report6()
        {
            var user = this._authService.CurrentUserData;
            var model = _webClient.DownloadData<Report6>("getreport6filter", null);
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if (user.Role.Equals("schooladmin", StringComparison.CurrentCultureIgnoreCase))
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (Resource.Reports_Schooladmin_Report5_Title).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            }
            return View("Report6", model);
        }

        /// <summary>
        ///  Report 6: Active Books in Kitablet :Get data for report
        /// </summary>
        [HttpPost]
        public ActionResult Report6(List<int> subsections, List<int> languages)
        {
            var model = _webClient.DownloadData<Report6>("getreport6", new { SubsectionIds = subsections != null ? string.Join(",", subsections) : "", LanguageIds = languages != null ? string.Join(",", languages) : "" });
            return PartialView("_Report6", model);
        }

        [HttpGet]
        public ActionResult ExportReport6()
        {
            return RedirectToRoute("Report6");
        }
        /// <summary>
        ///Report 6: Active Books in Kitablet: Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport6(string SubSectionIds = "", string LanguageIds = "")
        {
            var model = _webClient.DownloadData<List<Book>>("getreport6forexport", new { SubsectionIds = SubSectionIds, LanguageIds = LanguageIds });

            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport6ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report6.xlsx");
        }

        /// <summary>
        /// Report 7: Kitablet Usage Across Platforms and Environments : Get data for filter
        /// </summary>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]
        public ActionResult Report7()
        {
            var model = _webClient.DownloadData<Report7FilterModel>("getreport7fillters", null);
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report7", model);
        }
        /// <summary>
        ///  Report 7: Kitablet Usage Across Platforms and Environments :Get data for report
        /// </summary>
        [HttpPost]
        public ActionResult Report7(string schoolids, string duration, string city, string month, string year, string type)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (duration == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)), 23, 59, 59);
            }
            else if (duration == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month) + 2, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month) + 2), 23, 59, 59);
            }
            else if (duration == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(year), 1, 1);
                enddate = new DateTime(Convert.ToInt32(year), 12, 31, 23, 59, 59);
            }
            else if (duration == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            }
            var model = _webClient.DownloadData<Report7Model>("getreport7", new { StartDate = startdate, EndDate = enddate, SchoolIds = schoolids, City = city });

            if (type == "city")
            {

                return PartialView("_Report7Usage", model.UsageModel);
            }
            else if (type == "school")
            {

                return PartialView("_Report7Usage", model.UsageModel);
            }
            else
                return View("_Report7", model);
        }

        [HttpGet]
        public ActionResult ExportReport7()
        {
            return RedirectToRoute("Report7");
        }

        /// <summary>
        ///Report 7: Kitablet Usage Across Platforms and Environments: Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport7(string schoolidsexport, string durationexport, string cityexport, string monthexport, string yearexport)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (durationexport == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport)));
            }
            else if (durationexport == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2, DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2));
            }
            else if (durationexport == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), 1, 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), 12, 31);
            }
            else if (durationexport == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = DateTime.Now;
            }
            var model = _webClient.DownloadData<List<Report7ExportModel>>("getreport7export", new { StartDate = startdate, EndDate = enddate, SchoolIds = schoolidsexport, City = cityexport });
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport7ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report7.xlsx");

        }

        /// <summary>
        /// Report 2: School wise usage of kitablet by students : Get data for filter
        /// </summary>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin,schooladmin")]
        public ActionResult Report2()
        {
            var model = _webClient.DownloadData<Report2FilterModel>("getreport2fillters", new { UserId = _authService.CurrentUserData.UserId });
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report2", model);
        }

        /// <summary>
        ///  Report 2: School wise usage of kitablet by students :Get data for report
        /// </summary>
        [HttpPost]
        public ActionResult Report2(List<string> SchoolIds, List<string> grade, string duration, string city, string month, string year)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (duration == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)), 23, 59, 59);
            }
            else if (duration == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month) + 2, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month) + 2), 23, 59, 59);
            }
            else if (duration == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(year), 1, 1);
                enddate = new DateTime(Convert.ToInt32(year), 12, 31, 23, 59, 59);
            }
            else if (duration == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            }

            var model = _webClient.DownloadData<Report2>("getreport2", new { StartDate = startdate, EndDate = enddate, SchoolIds = SchoolIds != null ? string.Join(",", SchoolIds) : "", GradeIds = grade != null ? string.Join(",", grade) : "", City = city });

            return PartialView("_Report2", model);
        }

        /// <summary>
        /// Report 8: Usage of Kitablet by Your Children : Get data for filter
        /// </summary>
        [HttpGet]
        [FISEAuthorize(InRoles = "parent")]
        public ActionResult Report8()
        {
            var model = _webClient.DownloadData<List<StudentRegistrationModel>>("getreport8fillters", new { UserId = _authService.CurrentUserData.UserId });
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report8", model);
        }
        /// <summary>
        ///  Report 8:  Usage of Kitablet by Your Children :Get data for report
        /// </summary>
        [HttpPost]
        public ActionResult Report8(List<string> student, string duration, string month, string year)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (duration == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)), 23, 59, 59);
            }
            else if (duration == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month) + 2, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month) + 2), 23, 59, 59);
            }
            else if (duration == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(year), 1, 1);
                enddate = new DateTime(Convert.ToInt32(year), 12, 31, 23, 59, 59);
            }
            else if (duration == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            }
            var model = _webClient.DownloadData<List<Report8>>("getreport8", new { StartDate = startdate, EndDate = enddate, StudentIds = student != null ? string.Join(",", student) : "", UserId = _authService.CurrentUserData.UserId });

            return PartialView("_Report8", model);
        }
        /// <summary>
        ///Report 8: Usage of Kitablet by Your Children : Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport8(string studentsexport, string durationexport, string monthexport, string yearexport)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (durationexport == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport)));
            }
            else if (durationexport == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2, DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2));
            }
            else if (durationexport == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), 1, 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), 12, 31);
            }
            else if (durationexport == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = DateTime.Now;
            }
            var model = _webClient.DownloadData<List<Report8Export>>("getreport8export", new { StartDate = startdate, EndDate = enddate, StudentIds = studentsexport, UserId = _authService.CurrentUserData.UserId });
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport8ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report8.xlsx");
        }

        [HttpGet]
        public ActionResult ExportReport8()
        {
            return RedirectToRoute("Report8");
        }

        public ActionResult Report2StudentWise(string schoolid, string section, int grade, string duration, string month, string year, string type, int userid,string callfrom="")
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (duration == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)), 23, 59, 59);
            }
            else if (duration == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                enddate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month) + 2, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month) + 2), 23, 59, 59);
            }
            else if (duration == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(year), 1, 1);
                enddate = new DateTime(Convert.ToInt32(year), 12, 31, 23, 59, 59);
            }
            else if (duration == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            }
            ViewBag.schoolid = schoolid;
            var model = _webClient.DownloadData<Report2StudentWise>("getreport2studentwise", new { StartDate = startdate, EndDate = enddate, Section = section, UserId = userid, GradeId = grade, SchoolId = schoolid, CallFrom = callfrom });
            if (type == "report")
            {
                return PartialView("_Report8", model.StudentReport);
            }
            else
                return PartialView("_Report2StudentWise1", model);

        }

        public ActionResult Reports()
        {
            return View("ReportsListing");
        }

        [HttpGet]
        public ActionResult ExportReport2()
        {
            return RedirectToRoute("Report2");
        }

        /// <summary>
        ///Report 2: School wise usage of kitablet by students: Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport2(string schoolsexport, string gradesexport, string durationexport, string cityexport, string monthexport, string yearexport)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (durationexport == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport)));
            }
            else if (durationexport == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2, DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2));
            }
            else if (durationexport == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), 1, 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), 12, 31);
            }
            else if (durationexport == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = DateTime.Now;
            }

            var model = _webClient.DownloadData<List<Report2Export>>("getreport2export", new { StartDate = startdate, EndDate = enddate, SchoolIds = schoolsexport, GradeIds = gradesexport, City = cityexport });
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport2ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report2.xlsx");
        }

        [HttpGet]
        public ActionResult ExportReport3()
        {
            return RedirectToRoute("Report3");
        }

        /// <summary>
        ///Report 3:Book wise usage of kitablet by students: Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport3(string subsectionexport, string languageexport, string durationexport, string cityexport, string monthexport, string yearexport, string bookexport)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (durationexport == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport)));
            }
            else if (durationexport == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2, DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2));
            }
            else if (durationexport == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), 1, 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), 12, 31);
            }
            else if (durationexport == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = DateTime.Now;
            }
            var model = _webClient.DownloadData<List<Report3Export>>("getreport3export", new { StartDate = startdate, EndDate = enddate, LanguageIds = languageexport, SubsectionIds = subsectionexport, BookIds = bookexport, City = cityexport });

            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport3ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report3.xlsx");
        }

        [HttpGet]
        public ActionResult ExportReport4()
        {
            return RedirectToRoute("Report4");
        }


        /// <summary>
        ///Report 4: Book wise usage of kitablet by students for Publishers: Export report data in an excel
        /// </summary>
        [HttpPost]
        public ActionResult ExportReport4(string subsectionexport, string languageexport, string durationexport, string cityexport, string monthexport, string yearexport, string publisherexport)
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;
            if (durationexport == "a-month")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport)));
            }
            else if (durationexport == "a-quarter")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport), 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2, DateTime.DaysInMonth(Convert.ToInt32(yearexport), Convert.ToInt32(monthexport) + 2));
            }
            else if (durationexport == "a-year")
            {
                startdate = new DateTime(Convert.ToInt32(yearexport), 1, 1);
                enddate = new DateTime(Convert.ToInt32(yearexport), 12, 31);
            }
            else if (durationexport == "a-year-to-date")
            {
                startdate = DateTime.Now.AddYears(-1);
                enddate = DateTime.Now;
            }
            var model = _webClient.DownloadData<List<Report3Export>>("getreport4export", new { StartDate = startdate, EndDate = enddate, LanguageIds = languageexport, SubsectionIds = subsectionexport, Publisher = publisherexport, City = cityexport });

            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport4ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report4.xlsx");
        }

        [HttpGet]
        [FISEAuthorize(InRoles = "schooladmin")]
        public ActionResult Report1SchoolAdmin(int cpageno = 1, int pagesize = 0)
        {
            if (pagesize <= 0)
                pagesize = _pageSize;
            var apimodel = _webClient.DownloadData<Report1SchoolAdminResult>("getreport1schooladmin", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = cpageno });
            Report1SchoolAdmin model = new Report1SchoolAdmin();
            model.Created = new PagedList<StudentRegistrationModel>(apimodel.Created.Items, cpageno, pagesize, apimodel.Created.TotalItems);
            model.CreatedTotal = apimodel.CreatedTotal;
            model.Registered = new PagedList<StudentRegistrationModel>(apimodel.Registered.Items, cpageno, pagesize, apimodel.Registered.TotalItems);
            model.RegisteredTotal = apimodel.RegisteredTotal;
            model.Active = new PagedList<StudentRegistrationModel>(apimodel.Active.Items, cpageno, pagesize, apimodel.Active.TotalItems);
            model.ActiveTotal = apimodel.ActiveTotal;
            model.Grade = apimodel.Grade;
            model.Section = apimodel.Section;
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title.Substring(0, 20) + "...";
            return View("Report1SchoolAdmin", model);
        }

        [HttpPost]
        public ActionResult Report1SchoolAdmin(int cpageno = 1, int rpageno = 1, int apageno = 1, int cgradeid = 0, int rgradeid = 0, int agradeid = 0, string csection = "", string rsection = "", string asection = "", int pagesize = 0)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var target = Request.QueryString["target"];
            if (cgradeid != 0 || target=="created")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getreport1schooladminlist", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = cpageno, Type = "created", Section = csection, GradeId = cgradeid });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, cpageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1SchoolAdminList1", model);
            }
            else if (rgradeid != 0 || target == "registered")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getreport1schooladminlist", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = rpageno, Type = "registered", Section = rsection, GradeId = rgradeid });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, rpageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1SchoolAdminList2", model);
            }
            else if (agradeid != 0 || target == "active")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getreport1schooladminlist", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = apageno, Type = "active", Section = asection, GradeId = agradeid });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, apageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1SchoolAdminList3", model);
            }
            else
            {
                var apimodel = _webClient.DownloadData<Report1SchoolAdminResult>("getreport1schooladmin", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = cpageno });
                Report1SchoolAdmin model = new Report1SchoolAdmin();
                model.Created = new PagedList<StudentRegistrationModel>(apimodel.Created.Items, cpageno, pagesize, apimodel.Created.TotalItems);
                model.Registered = new PagedList<StudentRegistrationModel>(apimodel.Registered.Items, cpageno, pagesize, apimodel.Registered.TotalItems);
                model.Active = new PagedList<StudentRegistrationModel>(apimodel.Active.Items, cpageno, pagesize, apimodel.Active.TotalItems);
                model.Grade = apimodel.Grade;
                model.Section = apimodel.Section;
                return PartialView("Report1SchoolAdmin", model);
            }
        }

        [HttpPost]
        public ActionResult Report1SchoolAdminListAjax(int pageno = 1,  int gradeid = 0,  string section = "",  int pagesize = 0,string target="created")
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            if (target == "created")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getreport1schooladminlist", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = pageno, Type = "created", Section = section, GradeId = gradeid });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, pageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1SchoolAdminList1", model);
            }
            else if (target == "registered")
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getreport1schooladminlist", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = pageno, Type = "registered", Section = section, GradeId = gradeid });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, pageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1SchoolAdminList2", model);
            }
            else 
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getreport1schooladminlist", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = pagesize, PageIndex = pageno, Type = "active", Section = section, GradeId = gradeid });
                var model = new PagedList<StudentRegistrationModel>(apimodel.Items, pageno, pagesize, apimodel.TotalItems);
                return PartialView("_Report1SchoolAdminList3", model);
            }
        }

        [HttpGet]
        public ActionResult ExportReport1SchoolAdmin()
        {
            return RedirectToRoute("Report1SchoolAdmin");
        }

        [HttpPost]
        public ActionResult ExportReport1SchoolAdmin(string typeexport = "all")
        {
            List<StudentRegistrationModel> model = new List<StudentRegistrationModel>();
            if (typeexport == "all")
            {
                var apimodel = _webClient.DownloadData<Report1SchoolAdminResult>("getreport1schooladmin", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = int.MaxValue, PageIndex = 1, IsExport=true });
                model.InsertRange(0, apimodel.Created.Items);
                model.InsertRange(0, apimodel.Registered.Items);
                model.InsertRange(0, apimodel.Active.Items);
            }
            else
            {
                var apimodel = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getreport1schooladminlist", new { SchoolUId = _authService.CurrentUserData.SchoolUId, PageSize = int.MaxValue, PageIndex = 1, Type = typeexport, Section ="export", GradeId = 0 });
                model = apimodel.Items;
            }
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportReport1ToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "Report1.xlsx");
        }        
    }
}