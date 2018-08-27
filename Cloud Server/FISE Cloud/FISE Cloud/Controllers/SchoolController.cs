using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FISE_Cloud.Models.School;
using FISE_Cloud.Validators.School;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients;
using FISE_Cloud.Models;
using FISE_Cloud.Validators.Student;
using FISE_Cloud.Validators.User;
using Webdiyer.WebControls.Mvc;
using FISE_Cloud.Filters;
using System.Web;

namespace FISE_Cloud.Controllers
{
    [FISEAuthorize(InRoles = "superadmin,elibadmin")]
    [NoCache]
    /// <summary>
    /// This controller will handle requests related to school context
    /// </summary>
    public class SchoolController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;
        private readonly int _pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
        private readonly int _chunkSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["StudentImportUpdateChunkSize"]);
        public SchoolController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData != null ? _authService.CurrentUserData.UserId : 0);
        }

        #region Methods  
        /// <summary>
        /// Disable a school admin by making it Trashed.
        /// </summary>
        /// <returns>True/False</returns>      
        [HttpPost]
        public ActionResult DisableSchoolAdmin(int adminid)
        {
            var Result = _webClient.UploadData<SchoolAdminDisableStatus>("disableschooladmin", new { UserId = adminid });
            switch (Result)
            {
                case SchoolAdminDisableStatus.Success:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);
                case SchoolAdminDisableStatus.LastAdminDeletionNotallowed:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);
                case SchoolAdminDisableStatus.NotASchoolAdmin:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);

                default:
                    return Json(new { Status = SchoolAdminDisableStatus.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Disable a school by making it Trashed.
        /// </summary>
        /// <returns>True/False</returns>  
        [HttpPost]
        public ActionResult DisableSchool(int schoolid)
        {
            int _Result = 0;
            try
            {
                if (schoolid != 0)
                {
                    _Result = _webClient.UploadData<int>("disableschool", new { SchoolId = schoolid });
                }
            }
            catch { }
            return Json(new
            {
                Status = _Result
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Disable a Parent or student by making them Trashed.
        /// </summary>
        /// <returns>True/False</returns>  
        [HttpPost]
        public ActionResult DisableParentStudent(int userid)
        {
            bool _Result = false;
            try
            {
                if (userid != 0)
                {
                    _Result = _webClient.UploadData<bool>("disableparentstudent", new { UserId = userid });
                }
            }
            catch { }
            return Json(new
            {
                Status = _Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Render the School home page which will have school info, list of school admins and list of students
        /// </summary>
        [HttpGet]
        public ActionResult SchoolHomePage(string schooluid, int apageno = 1, int spageno = 1, int pagesize = 0)
        {
            var user = _authService.CurrentUser;
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }

            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "adminlist")
                {
                    var modeladminsapi = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("getschooladmins", new { PageIndex = apageno, PageSize = pagesize, SchoolUId = schooluid });
                    var Admins = new PagedList<UserRegistrationModel>(modeladminsapi.Items, spageno, pagesize, modeladminsapi.TotalItems);
                    return PartialView("_SchoolAdminListPost", Admins);
                }
                else if (target == "studentlist")
                {
                    var modelstudentapi = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getstudentsofschool", new { PageIndex = spageno, PageSize = pagesize, SchoolUId = schooluid });
                    var Students = new PagedList<StudentRegistrationModel>(modelstudentapi.Items, spageno, pagesize, modelstudentapi.TotalItems);
                    return PartialView("_StudentListPost", Students);
                }
                else
                {
                    var modelapi = _webClient.DownloadData<SchoolDetailsResult>("getschoolbyuid", new { PageIndex = apageno, PageSize = pagesize, SchoolUId = schooluid });
                    if (modelapi != null && modelapi.APIStatus != SchoolStatusEnum.NoSchoolFound)
                    {
                        SchoolDetails model = new SchoolDetails();
                        model.SchoolDetail = modelapi.SchoolDetails;
                        var Students = new PagedList<StudentRegistrationModel>(modelapi.Students.Items, spageno, pagesize, modelapi.Students.TotalItems);
                        var Admins = new PagedList<UserRegistrationModel>(modelapi.Admins.Items, apageno, pagesize, modelapi.Admins.TotalItems);
                        model.Admins = Admins;
                        model.Students = Students;
                        model.Grades = modelapi.Grades;
                        model.TotalStudents = modelapi.TotalStudents;
                        if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                        {
                            if ((model.SchoolDetail.SchoolName).Length > 20)
                                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.SchoolDetail.SchoolName).Substring(0, 20) + "...";
                            else
                                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.SchoolDetail.SchoolName;
                        }
                        return View(model);
                    }
                    else
                        return RedirectToRoute("PageNotFound");
                }
            }
            else
            {
                var modelapi = _webClient.DownloadData<SchoolDetailsResult>("getschoolbyuid", new { PageIndex = apageno, PageSize = pagesize, SchoolUId = schooluid });
                if (modelapi != null && modelapi.APIStatus != SchoolStatusEnum.NoSchoolFound)
                {
                    SchoolDetails model = new SchoolDetails();
                    model.SchoolDetail = modelapi.SchoolDetails;
                    var Students = new PagedList<StudentRegistrationModel>(modelapi.Students.Items, spageno, pagesize, modelapi.Students.TotalItems);
                    var Admins = new PagedList<UserRegistrationModel>(modelapi.Admins.Items, apageno, pagesize, modelapi.Admins.TotalItems);
                    model.Admins = Admins;
                    model.Students = Students;
                    model.Grades = modelapi.Grades;
                    model.TotalStudents = modelapi.TotalStudents;
                    if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                    {
                        if ((model.SchoolDetail.SchoolName).Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.SchoolDetail.SchoolName).Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.SchoolDetail.SchoolName;
                    }
                    return View(model);
                }
                else
                    return RedirectToRoute("PageNotFound");
            }

        }
        /// <summary>
        /// This is to handle the actions like paging and filtering posted from the school home page
        /// </summary>
        [HttpPost]
        public ActionResult SchoolHomePage(string schooluid, string Grade = "0", int apageno = 1, int spageno = 1, int pagesize = 0, string search = "")
        {
            var user = _authService.CurrentUser;
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "adminlist")
                {
                    var modeladminsapi = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("getschooladmins", new { PageIndex = apageno, PageSize = pagesize, SchoolUId = schooluid });
                    var Admins = new PagedList<UserRegistrationModel>(modeladminsapi.Items, apageno, pagesize, modeladminsapi.TotalItems);
                    return PartialView("_SchoolAdminListPost", Admins);
                }

                var modelstudentapi = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getstudentsofschool", new { PageIndex = spageno, PageSize = pagesize, SchoolUId = schooluid, Grade = Grade });
                var Students = new PagedList<StudentRegistrationModel>(modelstudentapi.Items, spageno, pagesize, modelstudentapi.TotalItems);
                return PartialView("_StudentListPost", Students);
            }
            else
            {
                var modelapi = _webClient.DownloadData<SchoolDetailsResult>("getschoolbyuid", new { PageIndex = apageno, PageSize = pagesize, SchoolUId = schooluid });
                SchoolDetails model = new SchoolDetails();
                model.SchoolDetail = modelapi.SchoolDetails;
                var Students = new PagedList<StudentRegistrationModel>(modelapi.Students.Items, spageno, pagesize, modelapi.Students.TotalItems);
                var Admins = new PagedList<UserRegistrationModel>(modelapi.Admins.Items, apageno, pagesize, modelapi.Admins.TotalItems);
                model.Admins = Admins;
                model.Students = Students;
                if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                {
                    if ((model.SchoolDetail.SchoolName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.SchoolDetail.SchoolName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.SchoolDetail.SchoolName;
                }
                return View(model);
            }
        }

        /// <summary>
        /// Resend the email varification email to school principle
        /// </summary>
        [HttpPost]
        public ActionResult ResendVerificationMail(int schoolid)
        {
            var Result = _webClient.UploadData<SchoolStatusEnum>("schoolregistrationemail", new { SchoolId = schoolid });
            switch (Result)
            {
                case SchoolStatusEnum.Success:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);

                case SchoolStatusEnum.NoSchoolFound:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);

                case SchoolStatusEnum.AlreadyHaveSchool:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);

                case SchoolStatusEnum.AccountNotFound:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);

                default:
                    return Json(new { Status = SchoolStatusEnum.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Export school details as an excel file
        /// </summary>
        [HttpPost]
        public ActionResult ExportSchoolHomePageDetailstoExcel(string schooluid_toexportschool)
        {
            var modelapi = _webClient.DownloadData<SchoolDetailsResult>("getschoolbyuid", new { PageIndex = 1, PageSize = int.MaxValue, SchoolUId = schooluid_toexportschool });
            SchoolDetails model = new SchoolDetails();
            List<SchoolDetails> model1 = new List<SchoolDetails>();
            model.SchoolDetail = modelapi.SchoolDetails;
            var Students = new PagedList<StudentRegistrationModel>(modelapi.Students.Items, 1, int.MaxValue, modelapi.Students.TotalItems);
            var Admins = new PagedList<UserRegistrationModel>(modelapi.Admins.Items, 1, int.MaxValue, modelapi.Admins.TotalItems);
            model.Admins = Admins;
            model.Students = Students;
            model.Grades = modelapi.Grades;
            model.TotalStudents = modelapi.TotalStudents;
            model1.Add(model); ;
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportSchoolHomePageDetailsToXlsx(model1);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "schooldetails.xlsx");
        }

        [HttpGet]
        public ActionResult ExportSchoolHomePageDetailstoExcel(string schooluid, string optional = "")
        {
            return RedirectToRoute("SchoolHomePage", new { schooluid = schooluid.ToString() });
        }

        /// <summary>
        /// Export list of school admins as an excel file
        /// </summary>
        [HttpPost]
        public ActionResult ExportSchoolAdminstoExcel(string schooluid_ofadmins)
        {
            var model = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("getschooladmins", new { PageIndex = 1, PageSize = int.MaxValue, SchoolUId = schooluid_ofadmins });
            var _exportManager = new Services.ExportImport.ExportManager();
            foreach (UserRegistrationModel schooladmin in model.Items)
            {
                if (!(schooladmin.Status) && schooladmin.IsTrashed)
                    schooladmin.Result = "Disabled";
                else if (schooladmin.Status && !(schooladmin.IsTrashed) && (schooladmin.LastLoginDate) != null)
                    schooladmin.Result = "ACTIVE";
                else if (schooladmin.Status && !(schooladmin.IsTrashed) && (schooladmin.LastLoginDate) == null)
                    schooladmin.Result = "REGISTERED";
                else if (!schooladmin.Status && !schooladmin.IsTrashed && (schooladmin.LastLoginDate) == null)
                    schooladmin.Result = "CREATED";
            }
            var bytes = _exportManager.ExportSchoolAdminsToXlsx(model.Items);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "librarians.xlsx");
        }

        [HttpGet]
        public ActionResult ExportSchoolAdminstoExcel(string schooluid, string optional = "")
        {
            return RedirectToRoute("SchoolHomePage", new { schooluid = schooluid.ToString() });
        }

        /// <summary>
        /// Export list of students of school as an excel file
        /// </summary>
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

        [HttpGet]
        public ActionResult ExportSchoolStudentstoExcel(string schooluid, string optional = "")
        {
            return RedirectToRoute("SchoolHomePage", new { schooluid = schooluid.ToString() });
        }
        /// <summary>
        /// Resend the registration email to a parent/ school admin
        /// </summary>
        [HttpPost]
        public ActionResult ResendUserRegistrationMail(int userid)
        {
            var Result = _webClient.UploadData<UserStatusEnum>("userregistrationemail", new { UserId = userid });
            switch (Result)
            {
                case UserStatusEnum.Success:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);
                case UserStatusEnum.UserAlreadyRegistered:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);
                case UserStatusEnum.WrongCredentails:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);
                case UserStatusEnum.UserAccountNotExist:
                    return Json(new { Status = Result }, JsonRequestBehavior.AllowGet);

                default:
                    return Json(new { Status = UserStatusEnum.Error }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public ActionResult CreateSchool()
        {
            var model = new School();
            return View(model);
        }
        /// <summary>
        /// Created new school 
        /// </summary>
        [HttpPost]
        public ActionResult CreateSchool(School model)
        {
            FluentValidation.IValidator<School> validator = new CreateSchoolValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var result = _webClient.UploadData<int>("createschool", model);
                switch (result)
                {
                    case 1:
                        model.Result = Resource.CreateSchool_createschool_success;
                        return View(model);
                    case 3:
                        ModelState.AddModelError("SchoolName", Resource.CreateSchool_SchoolAlreadyExists);
                        break;
                    case 5:
                        ModelState.AddModelError("SchoolUId", Resource.CreateSchool_SchoolUIdAlreadyExists);
                        break;
                    default:
                        ModelState.AddModelError("", Resource.PasswordRecovery_unknownerror);
                        break;
                }
            }


            return View(model);
        }

        /// <summary>
        /// Get school details with the help of school unique id to render the page for editing
        /// </summary>
        [HttpGet]
        public ActionResult EditSchool(string schooluid)
        {
            var result = _webClient.DownloadData<EditSchoolModel>("GetSchoolProfileDetails", new { SchoolUId = schooluid });
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((result.MySchool.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (result.MySchool.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = result.MySchool.SchoolName;
            }
            return View(result.MySchool);
        }

        /// <summary>
        /// Update school details
        /// </summary>
        [HttpPost]
        public ActionResult EditSchool(School model)
        {
            FluentValidation.IValidator<School> validator = new EditSchoolValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _webClient.UploadData<SchoolStatusEnum>("updateschool", model);
                    switch (result)
                    {
                        case SchoolStatusEnum.Success:
                            {
                                return RedirectToRoute("SchoolHomePage", new { schooluid = model.SchoolUId, pagesize = string.Empty, pageindex = string.Empty });
                            }
                        case SchoolStatusEnum.AlreadyHaveSchool:
                            ModelState.AddModelError("SchoolName", Resource.EditSchool_existMsg);
                            break;
                        case SchoolStatusEnum.NoSchoolFound:
                            ModelState.AddModelError("", Resource.EditSchool_noSchoolFound);
                            break;
                        case SchoolStatusEnum.AlreadyHaveSchoolUId:
                            ModelState.AddModelError("SchoolUId", Resource.CreateSchool_SchoolUIdAlreadyExists);
                            break;
                        default:
                            ModelState.AddModelError("", Resource.EditSchool_error);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if (model.SchoolName != null && (model.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.SchoolName;
            }
            return View(model);
        }

        /// <summary>
        /// Create a new school admin for a school
        /// </summary>
        [HttpGet]
        public ActionResult CreateSchoolAdmin(string schooluid)
        {
            UserCreationModel usermodel = new UserCreationModel();
            usermodel.Type = "schooladmin";
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((schooluid).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (schooluid).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = schooluid;
            }
            return View(usermodel);
        }

        /// <summary>
        /// Create a new school admin for a school
        /// </summary>
        [HttpPost]
        public ActionResult CreateSchoolAdmin(UserCreationModel usermodel, string schooluid)
        {
            usermodel.Type = "schooladmin";
            FluentValidation.IValidator<UserCreationModel> validator = new UserCreationValidator();
            var validationResults = validator.Validate(usermodel);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                usermodel.Email = usermodel.Email.Trim();
                usermodel.MobileNo = usermodel.MobileNo.Trim();

                string url = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority) + "/createschooladmin/usercreation";
                // on getting response from api about success or failure

                var result = _webClient.UploadData<UserStatusEnum>("addschooladmins", new { SchoolUId = schooluid, Email = usermodel.Email, MobileNo = usermodel.MobileNo, Url = url });
                switch (result)
                {
                    case UserStatusEnum.Success:
                        {
                            usermodel.Result = Resource.UserCreationFormConfirm_successmsg.Replace("%USERTYPE%", "School Librarian"); ;
                        }
                        break;
                    case UserStatusEnum.UserAlreadyRegistered:
                        ModelState.AddModelError("", Resource.UserCreation_UserAlreadyRegistered);
                        break;

                    default:
                        ModelState.AddModelError("", Resource.UserCreationForm_failuremsg);
                        break;
                }
            }
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((schooluid).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (schooluid).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = schooluid;
            }
            return View(usermodel);
        }
        /// <summary>
        /// Get school admin details
        /// </summary>
        [HttpGet]
        public ActionResult SchoolAdminInfo(string schooluid, string adminid)
        {
            int _adminid;
            if (!int.TryParse(adminid, out _adminid))
                return RedirectToRoute("PageNotFound");
            var model = _webClient.DownloadData<UserRegistrationModel>("getschooladmin", new { SchoolUId = schooluid, UserId = _adminid });
            if (model == null)
                return RedirectToRoute("PageNotFound");
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.SchoolName;
                if ((model.FirstName + " " + model.LastName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName;
            }
            return View(model);
        }

        /// <summary>
        /// Get school admin details with the help of school unique id and school admin id to render the page for editing
        /// </summary>
        [HttpGet]
        public ActionResult EditSchoolAdmin(string schooluid, string adminid)
        {
            int _adminid;
            if (!int.TryParse(adminid, out _adminid))
                return RedirectToRoute("PageNotFound");
            TempData["schooluID"] = schooluid;
            var model = _webClient.DownloadData<UserRegistrationModel>("getschooladmin", new { SchoolUId = schooluid, UserId = _adminid });
            if (model == null)
                return RedirectToRoute("PageNotFound");
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.SchoolName;
                if (String.IsNullOrEmpty(model.FirstName) && String.IsNullOrEmpty(model.LastName))
                {
                    if (model.Email.Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email.Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email;
                }
                else
                {
                    if ((model.FirstName + " " + model.LastName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.FirstName + " " + model.LastName;
                }
            }
            return View(model);
        }

        /// <summary>
        /// Update school admin details
        /// </summary>
        [HttpPost]
        public ActionResult EditSchoolAdmin(UserRegistrationModel model, string created)
        {
            if (string.IsNullOrEmpty(created))
            {
                FluentValidation.IValidator<UserRegistrationModel> validator = new SchoolAdminValidator();
                var validationResults = validator.Validate(model);
                foreach (var item in validationResults.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                try
                {
                    if (ModelState.IsValid)
                    {
                        UserProfile _User = new UserProfile();
                        _User.User = model;
                        _User.User.DobDate = 0;
                        _User.User.DobYear = 0;
                        _User.User.DobMonth = 0;
                        var result = _webClient.UploadData<GenericStatusEnum>("updateuserprofile", _User);
                        switch (result)
                        {
                            case GenericStatusEnum.Sucess:
                                {
                                    return RedirectToRoute("SchoolAdminInfo", new { schooluid = TempData["schooluID"], adminid = model.UserId });
                                }
                            default:
                                ModelState.AddModelError("", Resource.Edit_error);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                {
                    if ((model.SchoolName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.SchoolName;

                    if (String.IsNullOrEmpty(model.FirstName) && String.IsNullOrEmpty(model.LastName))
                    {
                        if (model.Email.Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email.Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email;
                    }
                    else
                    {
                        if ((model.FirstName + " " + model.LastName).Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.FirstName + " " + model.LastName;
                    }
                }
            }
            else
            {
                FluentValidation.IValidator<UserRegistrationModel> validator = new UserUpdateValidator();
                var validationResults = validator.Validate(model);
                foreach (var item in validationResults.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                try
                {
                    if (ModelState.IsValid)
                    {
                        var result = _webClient.UploadData<UserStatusEnum>("updatecreateduser", new { UserId = model.UserId, UserType = "schooladmin", Email = model.Email, MobileNo = model.MobileNumber });
                        switch (result)
                        {
                            case UserStatusEnum.Success:
                                {
                                    return RedirectToRoute("SchoolAdminInfo", new { schooluid = TempData["schooluID"], adminid = model.UserId });
                                }
                            case UserStatusEnum.UserAlreadyRegistered:
                                {
                                    ModelState.AddModelError("Email", Resource.UserCreation_UserAlreadyRegistered);
                                }
                                break;
                            case UserStatusEnum.UserAccountNotExist:
                                {
                                    ModelState.AddModelError("", Resource.UserCreation_UserAccNotExist);
                                }
                                break;
                            default:
                                ModelState.AddModelError("", Resource.Edit_error);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                {
                    if ((model.SchoolName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.SchoolName;
                    if (String.IsNullOrEmpty(model.FirstName) && String.IsNullOrEmpty(model.LastName))
                    {
                        if (model.Email.Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email.Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email;
                    }
                    else
                    {
                        if ((model.FirstName + " " + model.LastName).Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.FirstName + " " + model.LastName;
                    }
                }
            }
            return View(model);
        }
        /// <summary>
        /// Get details of a student along with his/her parent details
        /// </summary>
        [HttpGet]
        public ActionResult StudentandParentInfo(string schooluid, string studentid)
        {
            int _studentid;
            if (!int.TryParse(studentid, out _studentid))
                return RedirectToRoute("PageNotFound");
            TempData["schooluID"] = schooluid;

            var model = _webClient.DownloadData<StudentandParentInfo>("getstudentbyid", new { SchoolUId = schooluid, UserId = _studentid });
            if (model == null || model.APIStatus == 3)
                return RedirectToRoute("PageNotFound");

            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.SchoolName;
                if ((model.FirstName + " " + model.LastName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName;
            }

            return View(model);
        }

        /// <summary>
        /// Get parent details with the help of school id and userid to render the profile page for editing
        /// </summary>
        [HttpGet]
        public ActionResult EditParent(string schooluid, string userid)
        {
            int _userid;
            if (!int.TryParse(userid, out _userid))
                return RedirectToRoute("PageNotFound");
            TempData["schooluID"] = TempData["schooluID"];
            TempData["userid"] = TempData["userid"];
            var model = _webClient.DownloadData<UserProfile>("getparentprofile", new { UserId = _userid, SchoolUId = schooluid });
            if (model == null || model.Status == 0)
                return RedirectToRoute("PageNotFound");
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.User.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.User.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.User.SchoolName;
                if (String.IsNullOrEmpty(model.User.FirstName) && String.IsNullOrEmpty(model.User.LastName))
                {
                    if (model.User.Email.Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.User.Email.Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.User.Email;
                }
                else
                {
                    if ((model.User.FirstName + " " + model.User.LastName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.User.FirstName + " " + model.User.LastName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.User.FirstName + " " + model.User.LastName;
                }

            }
            return View(model.User);
        }

        /// <summary>
        ///Update parent details
        /// </summary>
        [HttpPost]
        [FISEAuthorize]
        public ActionResult EditParent(UserRegistrationModel model, string created)
        {
            if (string.IsNullOrEmpty(created))
            {
                FluentValidation.IValidator<UserRegistrationModel> validator = new EditUserProfileValidator();
                var validationResults = validator.Validate(model);
                foreach (var item in validationResults.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                if (ModelState.IsValid)
                {
                    model.DobDate = 0;
                    model.DobYear = 0;
                    model.DobMonth = 0;
                    var result = _webClient.UploadData<GenericStatusEnum>("updateuserprofile", new UserProfile() { User = model });

                    switch (result)
                    {
                        case GenericStatusEnum.Sucess:
                            {
                                return RedirectToRoute("StudentandParentInfo", new { schooluid = TempData["schooluID"], studentid = TempData["userid"] });
                            }
                        case GenericStatusEnum.Other:
                            ModelState.AddModelError("", Resource.EditUserProfile_Notfound);
                            break;
                        default:
                            ModelState.AddModelError("", Resource.EditUserProfile_UnknownErr);
                            break;
                    }
                }
                if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                {
                    if (model.SchoolName != null && (model.SchoolName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.SchoolName;

                    if (String.IsNullOrEmpty(model.FirstName) && String.IsNullOrEmpty(model.LastName))
                    {
                        if (model.Email.Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email.Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email;
                    }
                    else
                    {
                        if ((model.FirstName + " " + model.LastName).Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.FirstName + " " + model.LastName;
                    }
                }
            }
            else
            {
                FluentValidation.IValidator<UserRegistrationModel> validator = new UserUpdateValidator();
                var validationResults = validator.Validate(model);
                foreach (var item in validationResults.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }

                if (ModelState.IsValid)
                {
                    var result = _webClient.UploadData<UserStatusEnum>("updatecreateduser", new { UserId = model.UserId, UserType = "parent", Email = model.Email, MobileNo = model.MobileNumber });
                    switch (result)
                    {
                        case UserStatusEnum.Success:
                            {
                                return RedirectToRoute("StudentandParentInfo", new { schooluid = TempData["schooluID"], studentid = TempData["userid"] });
                            }
                        case UserStatusEnum.UserAlreadyRegistered:
                            {
                                ModelState.AddModelError("Email", Resource.UserCreation_UserAlreadyRegistered);
                            }
                            break;
                        case UserStatusEnum.UserAccountNotExist:
                            {
                                ModelState.AddModelError("", Resource.UserCreation_UserAccNotExist);
                            }
                            break;
                        default:
                            ModelState.AddModelError("", Resource.Edit_error);
                            break;
                    }
                }
                if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                {
                    if (model.SchoolName != null && (model.SchoolName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.SchoolName;
                    if (String.IsNullOrEmpty(model.FirstName) && String.IsNullOrEmpty(model.LastName))
                    {
                        if (model.Email.Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email.Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Email;
                    }
                    else
                    {
                        if ((model.FirstName + " " + model.LastName).Length > 20)
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                        else
                            MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.FirstName + " " + model.LastName;
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// Get student details with the help of school id and userid to render the profile page for editing
        /// </summary>
        [HttpGet]
        public ActionResult EditStudent(string schooluid, string studentid)
        {
            int _studentid;
            if (!int.TryParse(studentid, out _studentid))
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
                if ((model.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.SchoolName;
                if ((model.FirstName + " " + model.LastName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.FirstName + " " + model.LastName;
            }
            return View(model);
        }

        /// <summary>
        /// Update Student details
        /// </summary>
        [HttpPost]
        [FISEAuthorize]
        public ActionResult EditStudent(StudentProfile model)
        {
            FluentValidation.IValidator<StudentRegistrationModel> validator = new EditStudentValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }

            try
            {
                if (ModelState.IsValid)
                {

                    var result = _webClient.UploadData<UserStatusEnum>("updatestudent", model);

                    switch (result)
                    {
                        case UserStatusEnum.Success:
                            {
                                return RedirectToRoute("StudentandParentInfo", new { schooluid = model.SchoolUId, studentid = model.UserId });
                            }
                        case UserStatusEnum.UserAlreadyRegistered:
                            {
                                ModelState.AddModelError("", Resource.Editstudent_Duplicate);
                            }
                            break;
                        default:
                            ModelState.AddModelError("", Resource.Student_EditStudent_error);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.SchoolName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = (model.SchoolName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.ParentNode.Title = model.SchoolName;
                if ((model.FirstName + " " + model.LastName).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.FirstName + " " + model.LastName).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.FirstName + " " + model.LastName;
            }
            return View(model);
        }


        /// <summary>
        /// Add new students in a school by importing data from an excel file
        /// </summary>
        /// 
        [HttpGet]
        public ActionResult AddNewStudents()
        {
            return PartialView("_AddNewStudents");
        }

        public ActionResult ImportExportStudents()
        {
            var model = new StudentImportExportInput();
            model.ModalStatus = ImportExportModalstatus.Upload;
            return PartialView("_ImportExportStudents", model);
        }

        //*********************************
        /// <summary>
        /// Upload details of students to add in a school by importing data from an excel file
        /// </summary>
        [HttpPost]
        public ActionResult UploadStudents(string schooluidupload) /*ImportStudentsFromExcel()*/
        {
            //HttpContext.Server.ScriptTimeout = 1200;
            StudentImportExportInput model = new StudentImportExportInput();
            model.SchoolUId = schooluidupload;
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            ViewBag.FileName = file.FileName.ToString();
            try
            {
                List<StudentImportExport> students = new List<StudentImportExport>();
                var _importManager = new Services.ExportImport.ImportManager();
                if (file != null && file.ContentLength > 0)
                {
                    bool IsValidExcel;
                    List<StudentImportExport> studentimpexplist = _importManager.ImportStudentsFromXlsx(file.InputStream, out IsValidExcel);
                    if (!IsValidExcel)
                    {
                        model.ModalStatus = ImportExportModalstatus.InvalidExcel;
                        model.Students = students;
                        return PartialView("_ImportExportStudents", model);
                    }
                    students = studentimpexplist;
                    FluentValidation.IValidator<StudentImportExport> validator = new ImportStudentValidator();
                    for (int i = 0; i < students.Count; i++)
                    {
                        students[i].FirstName = string.IsNullOrEmpty(students[i].FirstName) ? " " : students[i].FirstName;
                        students[i].Grade = string.IsNullOrEmpty(students[i].Grade) ? " " : students[i].Grade;
                        students[i].ParentEmail = string.IsNullOrEmpty(students[i].ParentEmail) ? "e" : students[i].ParentEmail;
                        students[i].ParentMobileNumber = string.IsNullOrEmpty(students[i].ParentMobileNumber) ? "0" : students[i].ParentMobileNumber;
                        if (!string.IsNullOrEmpty(students[i].Gender))
                            students[i].Status = students[i].Gender.ToLower() == "male" || students[i].Gender.ToLower() == "female" ? true : false;
                        else
                            students[i].Status = true;
                        var validationResults = validator.Validate(students[i]);
                        //int dd = validationResults.Errors.Where(x => x.PropertyName.ToLower() == "dateofbirth").Count();
                        //if (dd != 0)
                        //{
                        //    var df = validationResults.Errors.Where(x => x.PropertyName.ToLower() == "dateofbirth").Select(y=>new FluentValidation.Results.ValidationFailure(y.PropertyName,y.ErrorMessage)).FirstOrDefault();
                        //    validationResults.Errors.Remove(df);
                        //}
                        students[i].Error = string.Join(", ", validationResults.Errors);
                    }
                    if (students.Count == 0)
                    {
                        model.ModalStatus = ImportExportModalstatus.Error;
                        model.Students = students;
                        return PartialView("_ImportExportStudents", model);
                    }
                    students = (from _student in students
                                where _student.Error != string.Empty
                                select _student
                                ).ToList();
                    if (students.Count > 0)
                    {
                        model.ModalStatus = ImportExportModalstatus.Error;
                        model.Students = students;
                        return PartialView("_ImportExportStudents", model);
                    }
                    else
                    {
                         //call api
                        //string data = Newtonsoft.Json.JsonConvert.SerializeObject(studentimpexplist);
                        var result = _webClient.UploadData<List<StudentImportExport>>("validateimportstudents", studentimpexplist,false);
                        students = (from _student in result
                                    where (_student.Status || _student.RowNumber > 1 || _student.GradeStatus)
                                    orderby _student.SNO
                                    select new StudentImportExport
                                    {
                                        SNO = _student.SNO,
                                        Error = (_student.Status && _student.RowNumber == 1 ? Resource.StudentImport_UserAlreadyReg_Error : "")
                                                + (_student.RowNumber > 1 ? ", " + Resource.StudentImport_Excelduplicate_Error : "")
                                                + (_student.GradeStatus ? ", " + Resource.StudentImport_InvalidGrade_Error : "")

                                    }
                                ).ToList();
                        if (students.Count > 0)
                        {
                            model.ModalStatus = ImportExportModalstatus.Error;
                            model.Students = students;
                        }
                        else
                        {
                            model.ModalStatus = ImportExportModalstatus.Import;
                            model.Students = result;
                        }
                        return PartialView("_ImportExportStudents", model);
                    }
                }
            }
            catch
            {
                model.ModalStatus = ImportExportModalstatus.ProcessError;
                return PartialView("_ImportExportStudents", model);
            }
            return View();
        }

        /// <summary>
        /// Send student data, imported from an excel file, to Web API
        /// </summary>

        [HttpPost]
        public ActionResult AddNewStudents(StudentImportExportInput model) /*ImportStudentsFromExcel()*/
        {
            var result = _webClient.UploadData<StudentImportExportInput>("importstudents", model, false);
            switch (result.Status)
            {
                case StudentsImportStatus.Sucess:
                    result.FailedCount = 0;
                    result.ProcessedStudents = model.Students.Select(c => { c.Status = true; return c; }).ToList();
                    break;
                default:
                    result.FailedCount = model.Students.Count;
                    result.ProcessedStudents = model.Students.Select(c => { c.Status = false; return c; }).ToList();
                    break;
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetImportSummery(StudentImportExportInput model) /*ImportStudentsFromExcel()*/
        {
            model.Students = model.Students.GroupBy(x => new { x.SubSection }).Select(g => new StudentImportExport { SubSection = g.Key.SubSection, TotalSuccess = g.Sum(x => x.TotalSuccess) }).ToList();
            model.ModalStatus = ImportExportModalstatus.Summery;
            return PartialView("_ImportExportStudents", model);
        }

        /// <summary>
        /// Export the failed students while updatting in an excel file
        /// </summary>
        //*****************************
        [HttpPost]
        public ActionResult ExportExcelFaliedToImportStudents(StudentImportExportInput model)
        {
            //HttpContext.Server.ScriptTimeout = 1200;
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportFaliedToImportStudentsToXlsx(model.ProcessedStudents);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "StudentImportReport.xlsx");
        }

        /// <summary>
        /// Update details of students of a school by importing data from an excel file
        /// </summary>
        //*****************************

        public ActionResult BulkUpdate()
        {
            var model = new StudentImportExportInput();
            model.ModalStatus = ImportExportModalstatus.Upload;
            return PartialView("_BulkUpdate", model);
        }
        /// <summary>
        /// upload details of students of a school by importing data from an excel file
        /// </summary>
        [HttpPost]
        public ActionResult UploadBulkUpdateStudents(string schooluidupload)
        {
            //HttpContext.Server.ScriptTimeout = 1200;
            StudentImportExportInput model = new StudentImportExportInput();
            model.SchoolUId = schooluidupload;
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            ViewBag.FileName = file.FileName.ToString();
            try
            {
                List<StudentImportExport> students = new List<StudentImportExport>();
                var _importManager = new Services.ExportImport.ImportManager();
                if (file != null && file.ContentLength > 0)
                {
                    bool IsValidExcel;
                    List<StudentImportExport> studentimpexplist = _importManager.BulkUpdateStudentsFromXlsx(file.InputStream, out IsValidExcel);
                    if (!IsValidExcel)
                    {
                        model.ModalStatus = ImportExportModalstatus.InvalidExcel;
                        model.Students = students;
                        return PartialView("_BulkUpdate", model);
                    }
                    students = studentimpexplist;
                    FluentValidation.IValidator<StudentImportExport> validator = new BulkUpdateStudentsValidator();
                    for (int i = 0; i < students.Count; i++)
                    {
                        students[i].IsRenew = students[i].IsRenew.ToLower() == "yes" || students[i].IsRenew.ToLower() == "no" ? students[i].IsRenew : "";
                        var validationResults = validator.Validate(students[i]);
                        students[i].Error = string.Join(", ", validationResults.Errors);
                    }
                    if (students.Count == 0)
                    {
                        model.ModalStatus = ImportExportModalstatus.Error;
                        model.Students = students;
                        return PartialView("_BulkUpdate", model);
                    }
                    students = (from _student in students
                                where _student.Error != string.Empty
                                select _student
                                ).ToList();
                    if (students.Count > 0)
                    {
                        model.ModalStatus = ImportExportModalstatus.Error;
                        model.Students = students;
                        return PartialView("_BulkUpdate", model);
                    }
                    else
                    {
                        //call api
                        var result = _webClient.UploadData<List<StudentImportExport>>("validatebulkupdatestudents", new { Student = studentimpexplist, SchoolUId = schooluidupload }, false);

                        students = (from _student in result
                                    where (_student.Status || _student.GradeStatus || _student.RowNumber > 1 || _student.StudentStatus || _student.IsAlreadyExists)
                                    orderby _student.SNO
                                    select new StudentImportExport
                                    {
                                        SNO = _student.SNO,
                                        Error = ((_student.Status && _student.RowNumber == 1 ? Resource.StudentImport_UserAccountNotExists.ToString() : "")
                                                + (_student.GradeStatus ? ", " + Resource.StudentImport_InvalidGrade_Error : "").Trim(',')
                                                + (_student.RowNumber > 1 ? ", " + Resource.StudentImport_Excelduplicate_Error : "")
                                                + (_student.StudentStatus && !_student.Status ? ", " + Resource.StudentRenew_WrongSchool : "")
                                                + (_student.IsAlreadyExists ? ", " + Resource.Editstudent_Duplicate.ToString() : "")).Trim(',')

                                    }
                                ).ToList();
                        if (students.Count > 0)
                        {
                            model.ModalStatus = ImportExportModalstatus.Error;
                            model.Students = students;
                        }
                        else
                        {
                            model.ModalStatus = ImportExportModalstatus.Import;
                            model.Students = result;
                        }
                        return PartialView("_BulkUpdate", model);
                    }
                }
            }
            catch
            {
                model.ModalStatus = ImportExportModalstatus.ProcessError;
                return PartialView("_BulkUpdate", model);
            }
            return View();
        }

        /// <summary>
        /// Update details of students of a school by importing data from an excel file
        /// </summary>
        ///        

        [HttpPost]
        public ActionResult BulkUpdateStudents(StudentImportExportInput model) /*ImportStudentsFromExcel()*/
        {
            var result = _webClient.UploadData<StudentImportExportInput>("updatemultiplestudents", model, false);
            switch (result.Status)
            {
                case StudentsImportStatus.Sucess:
                    result.FailedCount = 0;
                    result.ProcessedStudents = model.Students.Select(c => { c.Status = true; return c; }).ToList();
                    break;
                default:
                    result.FailedCount = model.Students.Count;
                    result.ProcessedStudents = model.Students.Select(c => { c.Status = false; return c; }).ToList();
                    break;
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUpdateSummery(StudentImportExportInput model)
        {
            model.Students = model.Students.GroupBy(x => new { x.SubSection }).Select(g => new StudentImportExport { SubSection = g.Key.SubSection, TotalSuccess = g.Sum(x => x.TotalSuccess) }).ToList();
            model.ModalStatus = ImportExportModalstatus.Summery;
            return PartialView("_BulkUpdate", model);
        }
        /// <summary>
        /// Export the failed students while updatting in an excel file
        /// </summary>
        //*****************************
        [HttpPost]
        public ActionResult ExportExcelFaliedToUpdateStudents(StudentImportExportInput model)
        {
            //  HttpContext.Server.ScriptTimeout = 1200;
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportFailedStudentsToXlsx(model.ProcessedStudents);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "StudentUpdateReport.xlsx");
        }
        /// <summary>
        /// Export the students of a school in an excel file
        /// </summary>
        //*****************************
        [HttpPost]
        public ActionResult ExportExcelSelectedStudents(string schooluidexport)
        {
            var model = _webClient.DownloadData<List<StudentImportExport>>("getstudentsofschoolforexport", new { PageIndex = 1, PageSize = int.MaxValue, SchoolUId = schooluidexport, Grade = "" });
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportStudentsToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(bytes, TextXlsx, "students.xlsx");
        }

        #region Lists
        /// <summary>
        /// This is to render the school list page
        /// </summary>
        /// 
        [HttpGet]
        public ActionResult SchoolList(int pageno = 1, int pagesize = 0)
        {
            var user = _authService.CurrentUser;
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var model = _webClient.DownloadData<APIPagedList<School>>("getschoollist", new { PageIndex = pageno, PageSize = pagesize, SearchText = "school5" });
            var pmodel = new PagedList<School>(model.Items, pageno, pagesize, model.TotalItems);
            return View(pmodel);
        }

        /// <summary>
        /// This is to handle the actions like paging and filtering posted from the school list page
        /// </summary>
        [HttpPost]
        public ActionResult SchoolList(string SearchText = "", int spageno = 1, int pagesize = 0)
        {
            var user = _authService.CurrentUser;
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var model = _webClient.DownloadData<APIPagedList<School>>("getschoollist", new { PageIndex = spageno, PageSize = pagesize, SearchText = SearchText });
            var pmodel = new PagedList<School>(model.Items, spageno, pagesize, model.TotalItems);
            if (Request.IsAjaxRequest())
                return PartialView("_SchoolListPost", pmodel);
            return View(pmodel);
        }

        [HttpGet]
        public ActionResult ExportSchoolstoExcel(string optional = "")
        {
            return RedirectToRoute("SchoolList");
        }

        /// <summary>
        /// Export the list of schools in an excel file
        /// </summary>
        [HttpPost]
        public ActionResult ExportSchoolstoExcel()
        {
            var model = _webClient.DownloadData<APIPagedList<School>>("getschoollist", new { PageIndex = 1, PageSize = int.MaxValue, SearchText = "" });
            var _exportManager = new Services.ExportImport.ExportManager();
            foreach (School school in model.Items)
            {
                if (school.IsTrashed)
                    school.Result = Resource.Status_Disabled;
                else if (school.IsEmailVerified && !school.IsTrashed)
                    school.Result = Resource.Status_Active;
                else if (!school.IsActive)
                    school.Result = Resource.Status_Created;
                else
                    school.Result = Resource.Status_Created;
            }
            var bytes = _exportManager.ExportSchoolsToXlsx(model.Items);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(bytes, TextXlsx, "schools.xlsx");
        }
        #endregion


        public ActionResult SchoolAdminDashboard(int pageno = 1, int pagesize = 0)
        {
            var user = _authService.CurrentUserData;
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var modelapi = _webClient.DownloadData<SchoolDetailsResult>("getschooladmindashboard", new { PageIndex = pageno, PageSize = pagesize, UserId = user.UserId });
            SchoolDetails model = new SchoolDetails();
            model.SchoolDetail = modelapi.SchoolDetails;
            var Students = new PagedList<StudentRegistrationModel>(modelapi.Students.Items, pageno, pagesize, modelapi.Students.TotalItems);
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

        [HttpPost]
        public ActionResult SchoolAdminDashboard(string schooluid, int apageno = 1, int spageno = 1, int pagesize = 0, string search = "", string Grade = "1")
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "adminlist")
                {
                    var modeladminsapi = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("getschooladmins", new { PageIndex = apageno, PageSize = pagesize, SchoolUId = schooluid });
                    var Admins = new PagedList<UserRegistrationModel>(modeladminsapi.Items, apageno, pagesize, modeladminsapi.TotalItems);
                    return PartialView("_SchoolAdminListPost", Admins);
                }

                var modelstudentapi = _webClient.DownloadData<APIPagedList<StudentRegistrationModel>>("getstudentsofschool", new { PageIndex = spageno, PageSize = pagesize, SchoolUId = schooluid, Grade = Grade });
                var Students = new PagedList<StudentRegistrationModel>(modelstudentapi.Items, spageno, pagesize, modelstudentapi.TotalItems);
                return PartialView("_StudentListPost", Students);
            }
            else
            {
                var modelapi = _webClient.DownloadData<SchoolDetailsResult>("getschoolbyuid", new { PageIndex = apageno, PageSize = pagesize, SchoolUId = schooluid });
                SchoolDetails model = new SchoolDetails();
                model.SchoolDetail = modelapi.SchoolDetails;
                var Students = new PagedList<StudentRegistrationModel>(modelapi.Students.Items, spageno, pagesize, modelapi.Students.TotalItems);
                var Admins = new PagedList<UserRegistrationModel>(modelapi.Admins.Items, apageno, pagesize, modelapi.Admins.TotalItems);
                model.Admins = Admins;
                model.Students = Students;
                if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
                {
                    if ((model.SchoolDetail.SchoolName).Length > 20)
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.SchoolDetail.SchoolName).Substring(0, 20) + "...";
                    else
                        MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.SchoolDetail.SchoolName;
                }

                return View(model);
            }
        }
        #endregion

        public ActionResult GetLastEmailSendDate(int UserId, string Type)
        {
            var date = _webClient.DownloadData<string>("getlastemailsenddate", new { UserId = UserId, Type = Type });
            return Json(date != "" ? Convert.ToDateTime(date).ToString("dd.MM.yyyy") : date, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ImportExportChildren()
        {
            var model = new ChildImportExportInput();
            model.ModalStatus = ImportExportModalstatus.Upload;
            return PartialView("_ImportExportChildren", model);
        }
        //*********************************
        /// <summary>
        /// Upload details of children to add in a school by importing data from an excel file
        /// </summary>
        [HttpPost]
        public ActionResult UploadChildren(string schooluidupload) /*ImportStudentsFromExcel()*/
        {
            ChildImportExportInput model = new ChildImportExportInput();
            model.SchoolUId = schooluidupload;
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            ViewBag.FileName = file.FileName.ToString();
            try
            {
                List<ChildImport> students = new List<ChildImport>();
                var _importManager = new Services.ExportImport.ImportManager();
                if (file != null && file.ContentLength > 0)
                {
                    bool IsValidExcel;
                    List<ChildImport> studentimpexplist = _importManager.ImportChildrensFromXlsx(file.InputStream, out IsValidExcel);
                    if (!IsValidExcel)
                    {
                        model.ModalStatus = ImportExportModalstatus.InvalidExcel;
                        model.Students = students;
                        return PartialView("_ImportExportChildren", model);
                    }
                    students = studentimpexplist;
                    FluentValidation.IValidator<ChildImport> validator = new ImportChildValidator();
                    for (int i = 0; i < students.Count; i++)
                    {
                        students[i].ChildFirstName = string.IsNullOrEmpty(students[i].ChildFirstName) ? " " : students[i].ChildFirstName;
                        students[i].Grade = string.IsNullOrEmpty(students[i].Grade) ? " " : students[i].Grade;
                        students[i].ParentEmailID = string.IsNullOrEmpty(students[i].ParentEmailID) ? "" : students[i].ParentEmailID;
                        students[i].ParentFirstName = string.IsNullOrEmpty(students[i].ParentFirstName) ? " " : students[i].ParentFirstName;
                        //if (!string.IsNullOrEmpty(students[i].Gender))
                        //    students[i].Status = students[i].Gender.ToLower() == "male" || students[i].Gender.ToLower() == "female" ? true : false;
                        //else
                        students[i].Status = true;
                        var validationResults = validator.Validate(students[i]);
                        students[i].Error = string.Join(", ", validationResults.Errors);
                    }
                    if (students.Count == 0)
                    {
                        model.ModalStatus = ImportExportModalstatus.Error;
                        model.Students = students;
                        return PartialView("_ImportExportChildren", model);
                    }
                    students = (from _student in students
                                where _student.Error != string.Empty
                                select _student
                                ).ToList();
                    if (students.Count > 0)
                    {
                        model.ModalStatus = ImportExportModalstatus.Error;
                        model.Students = students;
                        return PartialView("_ImportExportChildren", model);
                    }
                    else
                    {
                        //call api
                        var result = _webClient.UploadData<List<ChildImport>>("validateimportchildren", studentimpexplist, false);
                        students = (from _student in result
                                    where (_student.Status || _student.RowNumber > 1 || _student.GradeStatus)
                                    orderby _student.SNO
                                    select new ChildImport
                                    {
                                        SNO = _student.SNO,
                                        Error = (_student.Status && _student.RowNumber == 1 ? Resource.StudentImport_UserAlreadyReg_Error : "")
                                                + (_student.RowNumber > 1 ? ", " + Resource.StudentImport_Excelduplicate_Error : "")
                                                + (_student.GradeStatus ? ", " + Resource.StudentImport_InvalidGrade_Error : "")

                                    }
                                ).ToList();
                        if (students.Count > 0)
                        {
                            model.ModalStatus = ImportExportModalstatus.Error;
                            model.Students = students;
                        }
                        else
                        {
                            model.ModalStatus = ImportExportModalstatus.Import;
                            model.Students = result;
                        }
                        return PartialView("_ImportExportChildren", model);
                    }
                }
            }
            catch
            {
                model.ModalStatus = ImportExportModalstatus.ProcessError;
                return PartialView("_ImportExportChildren", model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult GetChildImportSummery(ChildImportExportInput model) /*ImportStudentsFromExcel()*/
        {
            model.Students = model.Students.GroupBy(x => new { x.SubSection }).Select(g => new ChildImport { SubSection = g.Key.SubSection, TotalSuccess = g.Sum(x => x.TotalSuccess) }).ToList();
            model.ModalStatus = ImportExportModalstatus.Summery;
            return PartialView("_ImportExportChildren", model);
        }
        /// <summary>
        /// Send student data, imported from an excel file, to Web API
        /// </summary>

        [HttpPost]
        public ActionResult AddNewChildren(ChildImportExportInput model) /*ImportChildrenFromExcel()*/
        {
            var result = _webClient.UploadData<ChildImportExportInput>("importchildren", model, false);
            switch (result.Status)
            {
                case StudentsImportStatus.Sucess:
                    result.FailedCount = 0;
                    result.ProcessedStudents = model.Students.Select(c => { c.Status = true; return c; }).ToList();
                    break;
                default:
                    result.FailedCount = model.Students.Count;
                    result.ProcessedStudents = model.Students.Select(c => { c.Status = false; return c; }).ToList();
                    break;
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Export the failed students while updatting in an excel file
        /// </summary>
        //*****************************
        [HttpPost]
        public ActionResult ExportExcelFaliedToImportChildren(ChildImportExportInput model)
        {
            //HttpContext.Server.ScriptTimeout = 1200;
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportFaliedToImportChildrenToXlsx(model.ProcessedStudents);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "ChildImportReport.xlsx");
        }
    }
}