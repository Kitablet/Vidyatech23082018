using FISE_Cloud.Models;
using FISE_Cloud.Services.Authentication;
using System.IO;
using System.Web.Mvc;

namespace FISE_Cloud.ViewEngines
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IAuthenticationService _authService;

        public override void InitHelpers()
        {
            base.InitHelpers();
            _authService = new FormsAuthenticationService();            
        }

        public IAuthenticationService FISEAuthenticationService 
        {
            get
            {
                return _authService;
            }
        }

        public User CurrentUser
        {
            get
            {                
                    return _authService.CurrentUser;
            }
        }

        public bool IsUserAuthenticated
        {
            get
            {
                return _authService.IsUserAuthenticated;
            }
        }

        public override string Layout
        {
            get
            {
                var layout = base.Layout;

                if (!string.IsNullOrEmpty(layout))
                {
                    var filename = Path.GetFileNameWithoutExtension(layout);
                    ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(ViewContext.Controller.ControllerContext, filename, "");

                    if (viewResult.View != null && viewResult.View is RazorView)
                    {
                        layout = (viewResult.View as RazorView).ViewPath;
                    }
                }

                return layout;
            }
            set
            {
                base.Layout = value;
            }
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {

    }
}