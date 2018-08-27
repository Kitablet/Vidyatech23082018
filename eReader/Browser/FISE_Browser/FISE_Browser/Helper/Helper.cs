using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FISE_Browser.TWebClients;
namespace FISE_Browser.Helper
{
    public class Helper
    {
        private   TWebClient _webClient;
        public Helper()
        {
            _webClient = new TWebClient();
        }

        public void UpdateBrowsing(string callfrom,string SessionId,DateTime StartTime,int UserId)
        {
            try
            {
               //var dd=  HttpContext.Current.Session["UserObject"];
               // System.Web.SessionState
              // HttpContext.Current.Session["starttime"] = DateTime.UtcNow;
                DateTime now = DateTime.UtcNow;
                int time = (int)(now - StartTime).TotalSeconds / 60;
                _webClient.UserId = UserId;
                var result = _webClient.UploadData<bool>("updatebrowsingtime", new { UserId = UserId, TimeSpent = time, EndDate = now, Environment = "Home", Plateform = "Browser", Callfrom = callfrom, SessionId = SessionId });
            }
            catch (Exception ex)
            {

            }
        }
    }
}