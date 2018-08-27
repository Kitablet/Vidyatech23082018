using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace FISE_Browser
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //if (authCookie != null)
            //{
            //    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //    UserData userData = null;
            //    try
            //    {
            //        userData = JsonConvert.DeserializeObject<UserData>(authTicket.UserData);
            //    }
            //    catch
            //    {
            //        if (!String.IsNullOrEmpty(authTicket.UserData))
            //        {
            //            //var WebClient = new LmsWebClient();
            //            //var result = WebClient.DownloadData<CustomerApiResult>("getcustomerbyusername", new { Username = authTicket.UserData, Role = role });
            //            //if (result.User != null)
            //            //{
            //            //    userData = new UserData
            //            //    {
            //            //        Role = result.User.Role,
            //            //        UserCacheKey = string.Format(CacheKey.USER_KEY, result.User.Id),
            //            //        SchoolId = result.User.SchoolId,
            //            //        UserId = result.User.Id,
            //            //        UserNameOrEmail = authTicket.UserData
            //            //    };
            //            //}
            //        }

            //    }
            //    AuthPrincipal newUser = new AuthPrincipal(authTicket);
            //    if (userData != null)
            //    {
            //        newUser.UserId = userData.UserId;
            //        newUser.Role = userData.Role;
            //        newUser.UserCacheKey = userData.UserCacheKey;
            //        newUser.SchoolId = userData.SchoolId;
            //    }
            //    HttpContext.Current.User = newUser;
            //}
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                FISE_Browser.Models.User userobj = (FISE_Browser.Models.User)Session["UserObject"];

                if (Session["RememberMe"]!=null &&  Convert.ToString( Session["RememberMe"]) == "1")
                {
                    // Session["SessionId"] = Session["SessionId"];
                    Session["RememberMe"] = Session["RememberMe"];
                    Session["UserObject"] = Session["UserObject"];
                }

                if (Session["starttime"] != null)
                {
                    DateTime _datetime = (DateTime)Session["starttime"];
                    if (Session["RememberMe"] != null && Convert.ToString(Session["RememberMe"]) == "1")
                    {
                        (new Helper.Helper()).UpdateBrowsing("timeout", userobj.SessionId, _datetime, userobj.UserId);
                        Session["starttime"] = DateTime.UtcNow;
                    }
                    else
                        (new Helper.Helper()).UpdateBrowsing("logout", userobj.SessionId, _datetime, userobj.UserId);
                    
                }
                else if (Session["RememberMe"] != null && Convert.ToString(Session["RememberMe"]) == "0")
                {
                    (new Helper.Helper()).UpdateBrowsing("deactivatesession", userobj.SessionId, DateTime.UtcNow, userobj.UserId);
                }
            }
            catch { }
        }

    }
}
