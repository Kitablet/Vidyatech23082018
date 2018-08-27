using FISE_Cloud.Filters;
using FISE_Cloud.Models;
using FISE_Cloud.Models.School;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients;
using FISE_Cloud.Validators.School;
using FISE_Cloud.Validators.User;
using System;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace FISE_Cloud.Controllers
{
    [FISEAuthorize(InRoles = "superadmin,elibadmin")]
    [NoCache]
    public class SuperAdminController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;
        private readonly int _pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
        public SuperAdminController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData != null ? _authService.CurrentUserData.UserId : 0);
        }        

        [ChildActionOnly]
        public ActionResult SuperAdminDashboard()
        {
            var model = _webClient.DownloadData<ElibSuperAdminDashboardResult>("getelibsuperadmindashboard", null);
            return PartialView("_SuperAdminDashboard", model);
        }


        [HttpPost]
        public ActionResult DisableElibraryAdmin(int elibadminid)
        {
            bool _Result = false;
            try
            {
                if (elibadminid != 0)
                {
                    _Result = _webClient.UploadData<bool>("disableelibadmin", new { UserId = elibadminid });
                }
            }
            catch { }
            return Json(new
            {
                Status = _Result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateElibraryAdmin()
        {
            UserCreationModel usermodel = new UserCreationModel();
            usermodel.Type = "elibraryadmin";
            return View(usermodel);
        }

        [HttpPost]
        public ActionResult CreateElibraryAdmin(UserCreationModel usermodel)
        {
            usermodel.Type = "elibraryadmin";
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
                var result = _webClient.UploadData<UserStatusEnum>("addelibadmin", new { Email = usermodel.Email, MobileNo = usermodel.MobileNo });  
                
                switch (result)
                {
                    case UserStatusEnum.Success:
                        {
                            usermodel.Result = Resource.UserCreationFormConfirm_successmsg.Replace("%USERTYPE%", "Elibrary Admin");
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

            return View(usermodel);
        }

        public ActionResult ElibraryAdminDashboard()
        {
            var model = new ElibraryAdminDashboard();
            model.TotalBookCount = 10;
            model.TotalSchoolCount = 4;
            model.TotalStudentCount = 16;
            return View(model);
        }

        [HttpGet]
        public ActionResult ElibraryAdminInfo(string elibadminid)
        {
            int _elibadminid;
            if(!int.TryParse(elibadminid,out _elibadminid))
                return RedirectToRoute("PageNotFound");
            var model = _webClient.DownloadData<UserRegistrationModel>("getelibraryadmin", new { UserId = _elibadminid });
            if (model == null)
                return RedirectToRoute("PageNotFound");
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if(!string.IsNullOrWhiteSpace(model.FirstName))
                MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.FirstName + " " + model.LastName;
            }
            return View(model);
        }

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
        public ActionResult EditElibraryAdmin(string elibadminid)
        {
            int _elibadminid;
            if (!int.TryParse(elibadminid, out _elibadminid))
                return RedirectToRoute("PageNotFound");
            var model = _webClient.DownloadData<ElibraryAdminRegistrationModel>("getelibraryadmin", new { UserId = _elibadminid });
            if (model == null)
                return RedirectToRoute("PageNotFound");

            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
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

        [HttpPost]
        public ActionResult EditElibraryAdmin(ElibraryAdminRegistrationModel model, string created)
        {
            if (string.IsNullOrEmpty(created))
            {
                FluentValidation.IValidator<ElibraryAdminRegistrationModel> validator = new ElibraryAdminValidator();
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
                                    return RedirectToRoute("ElibraryAdminInfo", new { elibadminid = model.UserId });
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
            else {
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
                        UserCreationModel _User = new UserCreationModel();
                        var result = _webClient.UploadData<UserStatusEnum>("updatecreateduser", new { UserId = model.UserId, UserType = "elibadmin",Email=model.Email,MobileNo=model.MobileNumber });                        
                        switch (result)
                        {
                            case UserStatusEnum.Success:
                                {
                                    return RedirectToRoute("ElibraryAdminInfo", new { elibadminid = model.UserId });
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

        #region Lists

        [HttpGet]
        public ActionResult ElibraryAdminList(int pageno = 1, int pagesize = 0)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var user = _authService.CurrentUser;
            var model = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("getelibraryadminslist", new { PageIndex = pageno, PageSize = pagesize, SearchText = "" });
            var pmodel = new PagedList<UserRegistrationModel>(model.Items, pageno, pagesize, model.TotalItems);
            return View(pmodel);                    
        }
        [HttpPost]
        public ActionResult ElibraryAdminList(string SearchText = "",int pageno = 1, int pagesize = 0)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var user = _authService.CurrentUser;
            var model = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("getelibraryadminslist", new { PageIndex = pageno, PageSize = pagesize, SearchText = SearchText });
            var pmodel = new PagedList<UserRegistrationModel>(model.Items, pageno, pagesize, model.TotalItems);
            if (Request.IsAjaxRequest())
                return PartialView("_ElibraryAdminListPost", pmodel);
            return View(pmodel);            
        }

        [HttpGet]
        public ActionResult ExportElibraryAdminstoExcel(string optional="")
        {
            return RedirectToRoute("ElibraryAdminList");
        }
        [HttpPost]
        public ActionResult ExportElibraryAdminstoExcel()
        {
            var model = _webClient.DownloadData<APIPagedList<UserRegistrationModel>>("getelibraryadminslist", new { PageIndex = 1, PageSize = int.MaxValue, SearchText = "" });
            var _exportManager = new Services.ExportImport.ExportManager();
            foreach (UserRegistrationModel elibadmin in model.Items)
            {
                if (!(elibadmin.Status) && elibadmin.IsTrashed)
                    elibadmin.Result = "Disabled";
                else if (elibadmin.Status && !(elibadmin.IsTrashed) && (elibadmin.LastLoginDate) != null)
                    elibadmin.Result = "ACTIVE";
                else if (elibadmin.Status && !(elibadmin.IsTrashed) && (elibadmin.LastLoginDate) == null)
                    elibadmin.Result = "REGISTERED";
                else if (!elibadmin.Status && !elibadmin.IsTrashed && (elibadmin.LastLoginDate) == null)
                    elibadmin.Result = "CREATED";
            }
            var bytes = _exportManager.ExportElibraryAdminsToXlsx(model.Items);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "elibraryadmins.xlsx");
        }

        #endregion
        
        public ActionResult ReportsListing()
        {
            return View();
        }
        public ActionResult RegistrationAndLoginReportFilter()
        {
            return View();
        }
        public ActionResult Report2aFilter()
        {
            return View();
        }
        public ActionResult Report3aFilter()
        {
            return View();
        }
        public ActionResult Report4aFilter()
        {
            return View();
        }
        public ActionResult Report5aFilter()
        {
            return View();
        }
        public ActionResult Report6aFilter()
        {
            return View();
        }
        public ActionResult Report7aFilter()
        {
            return View();
        }
        public ActionResult Report7b()
        {
            return View();
        }
    }
}