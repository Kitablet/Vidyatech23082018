using FISE_Cloud.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FISE_Cloud.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class FISEAuthorizeAttribute : AuthorizeAttribute
    {
        public string InRoles { get; set; }
        public string NotInRoles { get; set; }
        protected virtual AuthPrincipal CurrentMembar
        {
            get
            {
                return HttpContext.Current.User as AuthPrincipal;
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(this.InRoles))
                {
                    if (!CurrentMembar.IsInRole(this.InRoles))
                    {
                        HandleUnauthorizedRequest(filterContext);
                    }
                }
                if (!String.IsNullOrEmpty(this.NotInRoles))
                {
                    if (CurrentMembar.IsInRole(this.NotInRoles))
                    {
                        HandleUnauthorizedRequest(filterContext);
                    }
                }
            }
            else
            {
                HandleUnauthorizedRequest2(filterContext);
            }
        }

        /// <summary>
        /// If user is authenticated but not authorised to access the current page then user will be redirected to UnauthorizedAccess page
        /// </summary>
        private new void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult("UnauthorizedAccess", new RouteValueDictionary(new { action = "UnauthorizedAccess", controller = "Common" }));
        }
        /// <summary>
        /// If user is not authenticated then user will be redirected to Login page
        /// </summary>
        private void HandleUnauthorizedRequest2(AuthorizationContext filterContext)
        {
            string reurl = filterContext.HttpContext.Request.RawUrl;
            filterContext.Result = new RedirectToRouteResult("Login", new RouteValueDictionary(new { returnUrl = reurl }),true);
        }
    }
}
