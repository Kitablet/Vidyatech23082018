using FISE_Browser.Models;
using FISE_Browser.TWebClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FISE_Browser.Controllers
{
    public class BaseController : Controller
    {
        private readonly ITWebClient _webClient;

        public BaseController()
        {
            _webClient = new TWebClient();
        }
        public void CreateSession()
        {
            try
            {
                HttpCookie authCookie = Request.Cookies["userinfo"];
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
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    UserBase userData = null;

                    userData = JsonConvert.DeserializeObject<UserBase>(authTicket.UserData);
                    LoginObject loginObject = _webClient.UploadData<LoginObject>("browserlogin", new { Username = userData.Username, Password = userData.Password, Browser = Request.Browser.Browser, Plateform = osVersion, SessionId = userData.SessionId });
                    // Session["UserId"] = loginObject.User.UserId.ToString();
                    Session["UserObject"] = loginObject.User;
                    if (Session["starttime"] == null)
                        Session["starttime"] = DateTime.UtcNow;
                    // Session["SessionId"] = loginObject.User.SessionId;
                    Session["RememberMe"] = "1";

                }
                //else
                //{
                //    Session.Abandon();
                //}
            }

            catch { }
        }
    }
}