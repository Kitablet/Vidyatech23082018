using FISE_API.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace FISE_API
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
        }        
    }

    
}