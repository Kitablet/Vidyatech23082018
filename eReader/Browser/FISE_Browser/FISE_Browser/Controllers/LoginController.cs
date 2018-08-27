using FISE_Browser.Models;
using FISE_Browser.TWebClients;
using FISE_Browser.Validators.Home;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Security;
namespace FISE_Browser.Controllers
{
    public class LoginController : BaseController
    {
        int userid = 0;
        AvatarsModel avatarModel = new AvatarsModel();
        private TWebClient _webClient;
        private MainController _mainController;
        public LoginController()
        {
            _webClient = new TWebClient();
            _mainController = new MainController();
        }

        public ActionResult Index()
        {
            try
            {
                CreateSession();


                // HttpCookie reqCookies = Request.Cookies["userinfo"];
                //HttpCookie authCookie = Request.Cookies["userinfo"];
                //if (authCookie != null)
                //{
                //    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //    UserBase userData = null;
                //    try
                //    {
                //        userData = JsonConvert.DeserializeObject<UserBase>(authTicket.UserData);
                //        LoginObject loginObject = _webClient.UploadData<LoginObject>("browserlogin", new { Username = userData.Username, Password = userData.Password,SessionId="" });
                //        Session["UserId"] = loginObject.User.UserId.ToString();
                //        Session["UserObject"] = loginObject.User;
                //        Session["starttime"] = DateTime.Now;
                //        if (loginObject.User.AvatarId == 0)
                //        {
                //            return RedirectToAction("ChangeAvatar");
                //        }
                //        else
                //        {
                //            return RedirectToAction("Home", "Main"); ;
                //        }
                //    }
                //    catch { }
                //}

                User userobj = (User)Session["UserObject"];
                if (userobj != null)
                {
                    return RedirectToAction("Home", "Main");
                }
                else
                {
                    ViewBag.RememberMe = true;
                    return View("Index");
                }
            }
            catch (Exception ex) { return View("Index"); }
        }

        #region Avatar
        public ActionResult ChangeAvatar()
        {
            string avatarJson = string.Empty;
            List<Avatar> avatars = new List<Avatar>();
            if (Session["UserObject"] == null)
                _mainController.CreateSession();

            User userObject = (User)Session["UserObject"];
            try
            {
                userid = userObject.UserId; // Int32.Parse(Session["UserId"].ToString());
                avatars = _webClient.DownloadData<List<Avatar>>("GetAvatar", new { UserId = userid });
            }
            catch (Exception ex) { }
            if (avatars.Count > 0)
            {
                foreach (var item in avatars)
                {
                    string temp = item.ImagePath;
                    item.ImagePath = temp.Replace("#size#", "90X90");
                }
            }
            avatarModel.HasError = false;
            avatarModel.Avatars = avatars;

            if (userObject != null)
            {
                ViewData["UserName"] = userObject.FirstName + " " + userObject.LastName;
                return View(avatarModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult AddEditAvatar(int AvatarId, string AvatarUrl)
        {
            try
            {
                if (Session["UserObject"] == null)
                    _mainController.CreateSession();
                User userobj = (User)Session["UserObject"];

                if (userobj != null)
                {
                    bool resultContent = _webClient.UploadData<bool>("AddEditAvatar", new { UserId = userobj.UserId, AvatarId = AvatarId });
                    if (!resultContent)
                    {
                        userid = userobj.UserId; //Int32.Parse(Session["UserId"].ToString());
                        avatarModel.Avatars = _webClient.DownloadData<List<Avatar>>("GetAvatar", new { UserId = userid });
                        if (avatarModel.Avatars.Count > 0)
                        {
                            foreach (var item in avatarModel.Avatars)
                            {
                                string temp = item.ImagePath;
                                item.ImagePath = temp.Replace("#size#", "90X90");
                            }
                        }
                        avatarModel.HasError = true;
                        return View("ChangeAvatar", avatarModel);
                    }
                    else
                    {
                        userobj.AvatarId = AvatarId;

                        userobj.AvatarImage = AvatarUrl.Replace("90X90", "#size#"); ;
                        Session["UserObject"] = userobj;
                        return RedirectToAction("Home", "Main");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex) { }
            return View();
        }
        #endregion

        #region LOGIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserBase user, int RememberMe = 0)
        {
            ViewBag.RememberMe = RememberMe == 1 ? true : false;
            try
            {
                string osVersion = "";
                string userAgentText = Request.UserAgent;
                if (userAgentText != null)
                {
                    int startPoint = userAgentText.IndexOf('(') + 1;
                    int endPoint = userAgentText.IndexOf(';');

                    osVersion = userAgentText.Substring(startPoint, (endPoint - startPoint));
                }
                if (osVersion == "")
                    osVersion = Request.Browser.Platform;

                FluentValidation.IValidator<UserBase> validator = new UserLoginValidator();
                var validationResults = validator.Validate(user);
                foreach (var item in validationResults.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                if ((ModelState.IsValid) && ((!string.IsNullOrEmpty(user.Username)) && (!string.IsNullOrEmpty(user.Password))))
                {
                    LoginObject loginObject = _webClient.UploadData<LoginObject>("browserlogin", new { Username = user.Username, Password = user.Password, Browser = Request.Browser.Browser, Plateform = osVersion, SessionId = "", IPAddress = Request.UserHostAddress });

                    switch (loginObject.Status)
                    {
                        case 1:
                            if (RememberMe == 1)
                            {
                                user.UserId = loginObject.User.UserId;
                                user.SessionId = loginObject.User.SessionId;
                                var now = DateTime.UtcNow.ToLocalTime();
                                // HttpCookie userInfo = new HttpCookie("userInfo");
                                var userDataString = JsonConvert.SerializeObject(user);
                                var ticket = new FormsAuthenticationTicket(
                                                1 /*version*/,
                                                user.Username,
                                                now,
                                                DateTime.UtcNow.AddMonths(60),
                                                true,
                                                userDataString,
                                                FormsAuthentication.FormsCookiePath);

                                var encryptedTicket = FormsAuthentication.Encrypt(ticket);

                                var cookie = new HttpCookie("userinfo", encryptedTicket);
                                cookie.HttpOnly = true;
                                if (ticket.IsPersistent)
                                {
                                    cookie.Expires = ticket.Expiration;
                                }
                                cookie.Secure = FormsAuthentication.RequireSSL;
                                cookie.Path = FormsAuthentication.FormsCookiePath;
                                if (FormsAuthentication.CookieDomain != null)
                                {
                                    cookie.Domain = FormsAuthentication.CookieDomain;
                                }

                                Response.Cookies.Add(cookie);

                                //userInfo["UserName"] = user.Username;
                                //userInfo["Pass"] = user.Password;
                                //userInfo.Expires.Add(new TimeSpan(20000, 0, 1, 0));
                                //userInfo.HttpOnly = true;
                                //Response.Cookies.Add(userInfo);
                                Session["RememberMe"] = "1";
                            }
                            else
                                Session["RememberMe"] = "0";
                            //Session["UserId"] = loginObject.User.UserId.ToString();
                            Session["UserObject"] = loginObject.User;
                            Session["starttime"] = DateTime.UtcNow;
                            //Session["SessionId"] = loginObject.User.SessionId;
                            if (loginObject.User.AvatarId == 0)
                            {
                                return RedirectToAction("ChangeAvatar");
                            }
                            else
                            {
                                return RedirectToAction("Home", "Main"); ;
                            }
                        case 3:
                            ModelState.AddModelError("Username", Resource.PopupLogin_NoAccountError);
                            user.HasError = true;
                            break;
                        case 4:
                            ModelState.AddModelError("Password", Resource.WrongCredentials);
                            user.HasError = true;
                            break;
                        case 6:
                            ModelState.AddModelError("Username", Resource.AccountNotActive);
                            user.HasError = true;
                            break;
                        case 7:
                            ModelState.AddModelError("Username", Resource.AccountHasDisabled);
                            user.HasError = true;
                            break;
                        case 8:
                            ModelState.AddModelError("Username", Resource.NotAllowedToLogin);
                            user.HasError = true;
                            break;
                        case 9:
                            ModelState.AddModelError("Username", Resource.PopUpLogin_subscriptionExpired);
                            user.HasError = true;
                            break;
                        default:
                            ModelState.AddModelError("Username", Resource.PopUpLogin_unknownError);
                            user.HasError = true;
                            break;
                    }
                    return View("Index", user);
                }
                else { return View("Index"); }
            }
            catch (Exception ex) { return View("Index"); }

        }

        public ActionResult UserLogout()
        {
            try
            {
                try
                {
                    HttpCookie authCookie = Request.Cookies["userinfo"];
                    try
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        UserBase userData = null;
                        userData = JsonConvert.DeserializeObject<UserBase>(authTicket.UserData);

                        //FISE_Browser.Models.User userobj = (FISE_Browser.Models.User)Session["UserObject"];
                        DateTime _datetime = DateTime.UtcNow;
                        if (Session["starttime"] != null)
                            _datetime = (DateTime)Session["starttime"];
                        (new Helper.Helper()).UpdateBrowsing("logout", userData.SessionId, _datetime, userData.UserId);
                    }
                    catch { }
                    
                    if (authCookie != null)
                    {
                        Response.Cookies["userinfo"].Expires = DateTime.Now.AddDays(-1);
                    }
                }
                catch { }
                Session.Clear();
                return RedirectToAction("Home", "Main");
            }
            catch (Exception ex) { return RedirectToAction("Home", "Main"); }
            //After popup confirmation
            //Api Call
        }

        #endregion

        #region Recovery
        [HttpPost]
        public ActionResult RecoverPassword(string username)
        {

            int status = -1;
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(username))
            {
                status = _webClient.UploadData<int>("sendpasswordrecovery", new { Username = username, Url = string.Empty });
                switch (status)
                {
                    case 0:
                        msg = "Login failed due to technical reasons.";
                        break;
                    case 1:
                        msg = "Check your email to get the link to reset password.";
                        break;
                    case 2:
                        msg = "This user account does not exist.";
                        break;
                    case 3:
                        msg = "This user account is not active.";
                        break;
                    case 4:
                        msg = "This user account has been disabled.";
                        break;
                    case 6:
                        msg = "This user account is not active.";
                        break;
                    case 7:
                        msg = "This user account has been disabled.";
                        break;
                    case 8:
                        msg = "Sorry, you cannot login. Try Again.";
                        break;
                    case 9:
                        msg = "Subscription expired";
                        break;
                };
            }
            else
            {
                status = 0;
                msg = "Login failed due to technical reasons.";
            }
            return Json(new { Status = status, Message = msg });
        }
        [HttpPost]
        public ActionResult RecoverLoginName(string username)
        {
            int status = -1;
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(username))
            {
                //  var d = _webClient.DownloadData<int>("connectserver", null);
                status = _webClient.UploadData<int>("usernamerecovery", new { Email = username });
                switch (status)
                {
                    case 0:
                        msg = "Login failed due to technical reasons.";
                        break;
                    case 1:
                        msg = "Check your email to get the username.";
                        break;
                    case 2:
                        msg = "This user account does not exist.";
                        break;
                    case 3:
                        msg = "This user account is not active.";
                        break;
                    case 4:
                        msg = "This user account has been disabled.";
                        break;
                    case 6:
                        msg = "This user account is not active.";
                        break;
                    case 7:
                        msg = "This user account has been disabled.";
                        break;
                    case 8:
                        msg = "Sorry, you cannot login. Try Again.";
                        break;
                    case 9:
                        msg = "Subscription expired";
                        break;
                };
            }
            else
            {
                status = 0;
                msg = "Login failed due to technical reasons.";
            }
            return Json(new { Status = status, Message = msg });
        }
        #endregion
    }
}