using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FISE_Cloud.Controllers;
using System.Web.Security;
using Newtonsoft.Json;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients.Results;
using FISE_Cloud.TWebClients;
using FISE_Cloud.Caching;
using System.Configuration;

namespace FISE_Cloud
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                UserData userData = null;
                try
                {
                    userData = JsonConvert.DeserializeObject<UserData>(authTicket.UserData);
                }
                catch
                {
                    if (!String.IsNullOrEmpty(authTicket.UserData))
                    {
                        //var WebClient = new LmsWebClient();
                        //var result = WebClient.DownloadData<CustomerApiResult>("getcustomerbyusername", new { Username = authTicket.UserData, Role = role });
                        //if (result.User != null)
                        //{
                        //    userData = new UserData
                        //    {
                        //        Role = result.User.Role,
                        //        UserCacheKey = string.Format(CacheKey.USER_KEY, result.User.Id),
                        //        SchoolId = result.User.SchoolId,
                        //        UserId = result.User.Id,
                        //        UserNameOrEmail = authTicket.UserData
                        //    };
                        //}
                    }

                }
                AuthPrincipal newUser = new AuthPrincipal(authTicket);
                if (userData != null)
                {
                    newUser.UserId = userData.UserId;
                    newUser.Role = userData.Role;
                    newUser.UserCacheKey = userData.UserCacheKey;
                    newUser.SchoolUId = userData.SchoolUId;
                    newUser.DisplayName = userData.DisplayName;
                }
                HttpContext.Current.User = newUser;
            }
        }
        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);
            // If we're an ajax request, and doing a 302, then we actually need to do a 401
            if (Context.Response.StatusCode == 301 && context.Request.IsAjaxRequest())
            {
                Context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }

    }
}
