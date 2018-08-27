using System;
using System.Web.Mvc;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.Models;
using FISE_Cloud.TWebClients.Results;
using System.Net;
using FISE_Cloud.Filters;
using FISE_Cloud.TWebClients;
using FISE_Cloud.CustomResult;
using FISE_Cloud.Validators.User;
using CaptchaMvc.Attributes;
namespace FISE_Cloud.Controllers
{
    [NoCache]
    public class UserController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;

        public UserController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData != null ? _authService.CurrentUserData.UserId : 0);
        }

        #region Login/Logout

        [HttpGet]
        public ActionResult Login(string returnUrl, bool ShowRegMsg = false)
        {
            var user = _authService.CurrentUser;

            if (user != null)
                return RedirectToRoute("HomePage");
            var model = new LoginModel();
            model.ShowRegMsg = ShowRegMsg;
            model.AttemptCount = 1;
            return View(model);
        }

        [HttpPost, CaptchaVerify("Sorry, this Captcha is not valid. Try again.")]
        public ActionResult Login(LoginModel model, string returnUrl, string rememberme)
        {
            ModelState.Remove("AttemptCount");
            if (model.AttemptCount <= 1)
            {
                ModelState["CaptchaInputText"].Errors.Clear();
            }
            model.AttemptCount = model.AttemptCount + 1;
            FluentValidation.IValidator<LoginModel> validator = new LoginValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }           
            if (ModelState.IsValid)
            {
                model.Username = model.Username.Trim();
                model.Password = model.Password.Trim();
               
                try
                {
                    UserApiResult result = _webClient.UploadData<UserApiResult>("logincloud", model);
                    int status = result.Status;
                    switch (status)
                    {
                        case 1:
                            var customer = result.User;
                            customer.SchoolId = result.User.SchoolId;
                            _authService.SignIn(customer, rememberme!=null?true:false);
                            model.IsAuthenticated = true;
                            if (customer.Role.Equals("schooladmin", StringComparison.InvariantCultureIgnoreCase))
                                return RedirectToRoute("SchoolAdmin", new { schooluid = customer.SchoolUId });
                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        case 3:
                            ModelState.AddModelError("", Resource.PopupLogin_NoAccountError);
                            model.HasError = true;
                            break;
                        case 4:
                            ModelState.AddModelError("", Resource.PopUpLogin_wrongCredentials);
                            model.HasError = true;
                            break;
                        case 6:
                            ModelState.AddModelError("", Resource.Login_Accnotactive);
                            model.HasError = true;
                            break;
                        case 7:
                            ModelState.AddModelError("", Resource.Login_Accdisabled);
                            model.HasError = true;
                            break;
                        case 8:
                            ModelState.AddModelError("", Resource.Login_StudentsNotAllowedlogin);
                            model.HasError = true;
                            break;
                        default:
                            ModelState.AddModelError("", Resource.PopUpLogin_unknownError);
                            model.HasError = true;
                            break;

                    }
                }
                catch (WebException)
                {
                    ModelState.AddModelError("", Resource.PopUpLogin_NetError);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            _authService.SignOut();
            return RedirectToRoute("HomePage");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult IsUsernameUnique(string Username)
        {
            bool _Result = false;
            try
            {
                if (!string.IsNullOrEmpty(Username))
                {
                    _Result = _webClient.DownloadData<bool>("IsUsernameUnique", new { userName = Username });

                }
            }
            catch { }
            return Json(new
            {
                Status = _Result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Change Password
        [FISEAuthorize]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordModel();
            return View(model);
        }

        [HttpPost]
        [FISEAuthorize]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {

            FluentValidation.IValidator<ChangePasswordModel> validator = new ChangePasswordValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _webClient.UploadData<int>("changepassword", new { UserId = _authService.CurrentUserData.UserId, NewPassword = model.NewPassword, OldPassword = model.OldPassword });
                    switch (result)
                    {
                        case 1:
                            {
                                model.Result = Resource.Customer_ChangePassword_success;
                                break;
                            }
                        case 4:
                            ModelState.AddModelError("OldPassword", Resource.Customer_ChangePassword_oldPwdIncorrect);
                            break;
                        default:
                            ModelState.AddModelError("", Resource.Customer_ChangePassword_error);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }
        #endregion

        #region Password recovery

        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery"), CaptchaVerify("Sorry, this Captcha is not valid. Try again.")]
        [FormValueRequired("send-email")]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            FluentValidation.IValidator<PasswordRecoveryModel> validator = new PasswordRecoveryValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string url = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority) + "/passwordrecovery/confirm";
                    var result = _webClient.UploadData<int>("sendpasswordrecovery", new { Username = model.Username, Url = url });                    
                    switch (result)
                    {
                        case 1:
                            model.Result = Resource.PasswordRecovery_successmsg.ToString();
                            break;
                        case 3:
                            ModelState.AddModelError("", Resource.PasswordRecovery_msgnouser.ToString());
                            break;
                        default:
                            ModelState.AddModelError("", Resource.PasswordRecovery_unknownerror.ToString());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    model.Result = ex.Message;
                }

                return View(model);
            }
            //If we got this far, something failed, redisplay form
            return View(model);

        }

        public ActionResult PasswordRecoveryConfirm(string token)
        {
            if(_authService.CurrentUserData!=null&&_authService.CurrentUserData.UserId !=0)
                return RedirectToRoute("HomePage");
            string type = "passwordrecovery";
            var result = _webClient.UploadData<int>("validateusertoken", new { Token = token, Type = type });
            if (result == 1)
            {                
                var model = new PasswordRecoveryConfirmModel();
                model.Username = _webClient.DownloadData<string>("getusernamebytoken", new { token = token });
                return View(model);
            }
            else if (result == 3)
            {
                var model = new PasswordRecoveryConfirmModel() { Result = Resource.PasswordRecoveryConfirm_invalidtoken.Replace("%SITEURL%",Url.RouteUrl("PasswordRecovery")) };
                return View(model);
            }
            else
            {
                return RedirectToRoute("HomePage");
            }
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm"), CaptchaVerify("Sorry, this Captcha is not valid. Try again.")]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string username, PasswordRecoveryConfirmModel model)
        {
            FluentValidation.IValidator<PasswordRecoveryConfirmModel> validator = new PasswordRecoveryConfirmValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _webClient.UploadData<UserStatusEnum>("passwordrecoverysubmit", new { NewPassword = model.NewPassword, Token = token });
                    switch (result)
                    {
                        case UserStatusEnum.Success:
                            model.Result = Resource.PasswordRecoveryConfirm_successmsg;
                            model.SuccessfullyChanged = true;
                            return RedirectToAction("PasswordRecoveryConfirmResult");
                        case UserStatusEnum.UserAccountNotExist:
                            model.Result = Resource.PasswordRecoveryConfirm_noaccount;
                            break;
                        case UserStatusEnum.InvalidToken:
                            model.Result = Resource.PasswordRecoveryConfirm_invalidtoken.Replace("%SITEURL%", Url.RouteUrl("PasswordRecovery"));
                            break;
                        default:
                            return RedirectToRoute("HomePage");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            //redisplay page if any error
            return View(model);

        }

        public ActionResult PasswordRecoveryConfirmStudent(string token)
        {
            if (_authService.CurrentUserData != null && _authService.CurrentUserData.UserId != 0)
                return RedirectToRoute("HomePage");
            string type = "SchoolStudentsPasswordRecoveryEmail";
            var result = _webClient.UploadData<int>("validateusertoken", new { Token = token, Type = type });
            if (result == 1)
            {
                var model = new PasswordRecoveryConfirmModel();
                model.Username = _webClient.DownloadData<string>("getusernamebytoken", new { token = token });
                return View(model);
            }
            else if (result == 3)
            {
                var model = new PasswordRecoveryConfirmModel() { Result = Resource.PasswordRecoveryConfirm_invalidtoken.Replace("%SITEURL%", Url.RouteUrl("PasswordRecoveryConfirmStudent")) };
                return View(model);
            }
            else
            {
                return RedirectToRoute("HomePage");
            }
        }

        [HttpPost, ActionName("PasswordRecoveryConfirmStudent"), CaptchaVerify("Sorry, this Captcha is not valid. Try again.")]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordRecoveryConfirmStudentPOST(string token, string username, PasswordRecoveryConfirmModel model)
        {
            FluentValidation.IValidator<PasswordRecoveryConfirmModel> validator = new PasswordRecoveryConfirmStudentValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _webClient.UploadData<UserStatusEnum>("passwordrecoverysubmit", new { NewPassword = model.NewPassword, Token = token });
                    switch (result)
                    {
                        case UserStatusEnum.Success:
                            model.Result = Resource.PasswordRecoveryConfirm_successmsg;
                            model.SuccessfullyChanged = true;
                            return RedirectToAction("PasswordRecoveryConfirmResult");
                        case UserStatusEnum.UserAccountNotExist:
                            model.Result = Resource.PasswordRecoveryConfirm_noaccount;
                            break;
                        case UserStatusEnum.InvalidToken:
                            model.Result = Resource.PasswordRecoveryConfirm_invalidtoken.Replace("%SITEURL%", Url.RouteUrl("PasswordRecoveryConfirmStudent"));
                            break;
                        default:
                            return RedirectToRoute("HomePage");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            //redisplay page if any error
            return View(model);

        }

        public ActionResult PasswordRecoveryConfirmResult()
        {
            if (_authService.CurrentUser != null)
                return RedirectToRoute("HomePage");

            var model = new PasswordRecoveryConfirmModel();
            model.Result = Resource.PasswordRecoveryConfirm_successmsg;
            model.SuccessfullyChanged = true;
            return View("PasswordRecoveryConfirm", model);
        }

        #endregion

        public ActionResult UsernameRecovery()
        {
            var model = new UsernameRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("UsernameRecovery"), CaptchaVerify("Sorry, this Captcha is not valid. Try again.")]
        // [FormValueRequired("send-email")]
        [ValidateAntiForgeryToken]
        public ActionResult UsernameRecovery(UsernameRecoveryModel model)
        {
            FluentValidation.IValidator<UsernameRecoveryModel> validator = new UsernameRecoveryValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _webClient.UploadData<int>("usernamerecovery", new { Email = model.Email });

                    // var result = _webClient.UploadData<int>("", new { Email = model.Email, Url = url });

                    //var result = 1;
                    switch (result)
                    {
                        case 1:
                            model.Result = Resource.UsernameRecovery_successmsg.ToString();
                            break;
                        case 2:
                            ModelState.AddModelError("", Resource.UsernameRecovery_msgnouser.ToString());
                            break;
                        case 3:
                            ModelState.AddModelError("", Resource.UsernameRecovery_msguseraccnotactive);
                            break;
                        case 4:
                            ModelState.AddModelError("", Resource.UsernameRecovery_msguseraccdisabled);
                            break;
                        default:
                            ModelState.AddModelError("", Resource.UsernameRecovery_unknownerror.ToString());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    model.Result = ex.Message;
                }

                return View(model);
            }
            //If we got this far, something failed, redisplay form
            return View(model);

        }
        #region User Registeration Except Student

        [HttpGet]
        public ActionResult UserRegistration(string token)
        {
            //var result = 1;  // for testing purpose
            //if (result == 1)
            //{
            //    var model = new UserCreationModel();
            //    model.Email = "someone@gmail.com";
            //    model.GivenMobileNo = "1234567890".Substring(10 - 4, 4);
            //    return View(model);
            //}

            if (_authService.CurrentUserData != null && _authService.CurrentUserData.UserId != 0)
            {
                return RedirectToRoute("HomePage");
            }

            var result = _webClient.DownloadData<User>("getregisteration", new { Token = token });

            if (result.returnStatus == 1)
            {
                var model = new UserCreationModel();
                model.Email = result.Email;
                model.GivenMobileNo = result.MobileNumber.Substring(result.MobileNumber.Length-4, 4);
                model.UserId = result.UserId;
                model.Token = token;
                model.Type = result.Type;
                model.MobileNumber = result.MobileNumber;
                model.IsInvalidToken = false;
                model.AccountIsDisabled = false;
                return View(model);
            }
            if (result.returnStatus ==7)
            {
                var model = new UserCreationModel();                
                model.IsInvalidToken = false;
                model.AccountIsDisabled = true;
                return View(model);
            }
            else
            {
                var model = new UserCreationModel();
                model.IsInvalidToken = true;
                model.AccountIsDisabled = false;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult UserValidation(UserCreationModel model)
        {

            FluentValidation.IValidator<UserCreationModel> validator = new GetRegistrationValidator(model.MobileNumber.Length-4);
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            model.MobileNo = model.MobileNo + model.GivenMobileNo;
            if (ModelState.IsValid)
            {
                var result = _webClient.UploadData<User>("VerifyMobile", new { MobileNo = model.MobileNo, Email = model.Email, Type = model.Type, EntityId = model.UserId });

                if (result.returnStatus == 1)
                {
                    UserRegistrationModel model1 = new UserRegistrationModel();
                    model1.Token = model.Token;
                    model1.MobileNumber = model.MobileNo;
                    model1.Type = model.Type.ToLower();
                    //if type schoolregistration return thank you partial view
                    if (model.Type.ToLower() == "schoolregistration" || model.Type.ToLower() == "useremailchange")
                    {
                        ViewBag.IsPrinciple = model.Type.ToLower() == "schoolregistration" ? "true" : "false";
                        return PartialView("_EmailVerificationConfirmation");
                    }

                    return PartialView("UserRegistrationForm", model1);
                }
                else
                {
                    ModelState.AddModelError("MobileNo", Resource.UserValidation_MobileNoMismatch);
                    ModelState.AddModelError(String.Empty, Resource.UserValidation_MobileNoMismatch);
                    return PartialView(model);
                }
            }
            else
                return PartialView(model);
        }


        [HttpPost, CaptchaVerify("Sorry, this Captcha is not valid. Try again.")]
        public ActionResult UserRegistrationForm(UserRegistrationModel model)
        {
            int Id = model.UserId;
            string email = model.Email;
            string mobileno = model.MobileNumber;
            model.DobDate = DateTime.Now.Day;
            model.DobMonth = DateTime.Now.Month;
            model.DobYear = DateTime.Now.Year;
            FluentValidation.IValidator<UserRegistrationModel> validator = new UserRegistrationValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _webClient.UploadData<int>("register", model);
                    switch (result)
                    {
                        case 1:
                            return RedirectToAction("Login", "User", new { ShowRegMsg = true });
                        case 2:
                            ModelState.AddModelError("", Resource.UserCreation_UserNameAlreadyRegistered);
                            break;
                        case 4:
                            ModelState.AddModelError("", Resource.PopUpLogin_wrongCredentials);
                            break;
                        case 5:
                            model.IsInvalidToken = true;
                            break;
                        default:
                            ModelState.AddModelError("", Resource.UserCreation_error);
                            break;
                    }
                }
                catch
                {
                }
                return PartialView(model);
            }

            return PartialView(model);
        }

        #endregion

        
        [FISEAuthorize]
        public ActionResult UserProfileInfo()
        {
            var userdata = this._authService.CurrentUserData;
            if (userdata == null)
                return new FiseHttpUnauthorisedResult();

            var model = _webClient.DownloadData<UserProfile>("getuserbyid", new { UserId = userdata.UserId });
            if (model.Status == 1)
            {
                return View(model.User);
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditUserProfileInfo()
        {
            var userdata = this._authService.CurrentUserData;
            if (userdata == null)
                return new FiseHttpUnauthorisedResult();
            
            var model = _webClient.DownloadData<UserProfile>("getuserbyid", new { UserId = userdata.UserId });            
            ViewBag.OldEmail = model.User.Email;
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.User.Username).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.User.Username).Substring(0, 20);
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.User.Username;
            }
            return View(model.User);
        }

        [HttpPost]
        [FISEAuthorize]
        public ActionResult EditUserProfileInfo(UserRegistrationModel model,string OldEmail)
        {
            ViewBag.OldEmail = OldEmail;
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
                var result = _webClient.UploadData<UserStatusEnum>("updateuserprofile", new UserProfile() { User = model });

                switch (result)
                {
                    case UserStatusEnum.Success:
                        {
                            if (!OldEmail.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) )
                            {
                                _authService.SignOut();
                                return RedirectToRoute("HomePage");
                            }

                            var result1 = _webClient.DownloadData<UserProfile>("getuserbyid", new { UserId = _authService.CurrentUserData.UserId });
                            _authService.SignIn(result1.User, true );
                            return RedirectToRoute("UserProfileInfo");
                        }
                    case UserStatusEnum.UserAccountNotExist:
                        ModelState.AddModelError("APIError", Resource.EditUserProfile_Notfound);
                        break;
                    case UserStatusEnum.UserAlreadyRegistered:
                        ModelState.AddModelError("Email", Resource.UpdateProfile_EmailDup_Error);
                        break;
                    default:
                        ModelState.AddModelError("APIError", Resource.EditUserProfile_UnknownErr);
                        break;
                }
            }
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.Username).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.Username).Substring(0, 20);
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Username;
            }
            return View(model);
        }
        
    }
}