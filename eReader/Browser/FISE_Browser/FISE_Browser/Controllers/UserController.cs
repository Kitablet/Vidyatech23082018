using FISE_Browser.Models;
using FISE_Browser.TWebClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json.Linq;
namespace FISE_Browser.Controllers
{
    public class UserController : BaseController
    {
        private readonly TWebClient _webClient;


        public UserController()
        {
            _webClient = new TWebClient();
        }

        public ActionResult Index()
        {
            UserAvatar userAvatar = new UserAvatar();
            try
            {
                if (Session["UserObject"] == null)
                    CreateSession();
                User userobj = (User)Session["UserObject"];
                string avatarJson = string.Empty;
                List<Avatar> avatars = new List<Avatar>();
                if (userobj != null)
                {
                    _webClient.UserId = userobj.UserId;
                    BooksDetail booksobj = _webClient.DownloadData<BooksDetail>("getbookscatlog", null);
                    UserDetails result = (new TWebClient()).DownloadData<UserDetails>("getuserdata", new { UserId = userobj.UserId });
                    try
                    {
                        avatars = (new TWebClient()).DownloadData<List<Avatar>>("GetAvatar", new { UserId = userobj.UserId });
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
                    result.AllBooks = booksobj;
                    userobj.AllBooks = booksobj;
                    userAvatar.userObject = userobj;
                    userAvatar.avatars = avatars;
                    userAvatar.userObject.UserDetails = result;
                    return View(userAvatar);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Main");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string NewPassword, string OldPassword, int UserId)
        {
            int status = 0;
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            if (userobj != null && userobj.UserId == UserId)
            {
                try
                {
                    _webClient.UserId = userobj.UserId;
                    status = _webClient.UploadData<int>("changepassword", new { NewPassword = NewPassword, OldPassword = OldPassword, UserId = UserId });
                    HttpCookie authCookie = Request.Cookies["userinfo"];
                    if (authCookie != null)
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                        UserBase userData = JsonConvert.DeserializeObject<UserBase>(authTicket.UserData);
                        userData.Password = NewPassword;
                        var now = DateTime.UtcNow.ToLocalTime();
                        // HttpCookie userInfo = new HttpCookie("userInfo");
                        var userDataString = JsonConvert.SerializeObject(userData);
                        var ticket = new FormsAuthenticationTicket(
                                        1 /*version*/,
                                        userData.Username,
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
                    }
                }
                catch (Exception ex) { }
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeAvatar(int AvatarId, int UserId, string AvatarUrl)
        {
            bool status = false;
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            try
            {
                if (userobj != null && userobj.UserId == UserId)
                {
                    _webClient.UserId = userobj.UserId;
                    status = _webClient.UploadData<Boolean>("AddEditAvatar", new { AvatarId = AvatarId, UserId = UserId });
                    if (status)
                    {
                        userobj.AvatarId = AvatarId;
                        AvatarUrl = AvatarUrl.Replace("90X90", "#size#");
                        userobj.AvatarImage = AvatarUrl;
                        Session["UserObject"] = userobj;
                    }
                }
            }
            catch (Exception ex) { }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsSessionTimeOut()
        {
            if (Session["UserObject"] == null)
                CreateSession();

            if (Session["UserObject"] != null)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsSessionTimeOut1()
        {
            if (Session["UserObject"] == null)
                CreateSession();
            JObject data = new JObject();
            data.Add("starttime", DateTime.UtcNow.ToString());
            string satrttime = DateTime.UtcNow.ToString();
            if (Session["UserObject"] != null)
                data.Add("status", false);
            else
                data.Add("status", true);
            // return Json(data, JsonRequestBehavior.AllowGet);
            //return Json(true, JsonRequestBehavior.AllowGet);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

    }
}