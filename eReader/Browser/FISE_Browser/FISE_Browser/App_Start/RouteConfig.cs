using System.Web.Mvc;
using System.Web.Routing;

namespace FISE_Browser
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Error1",
               url: "Error",
               defaults: new { controller = "Error", action = "Error", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Error2",
               url: "PageNotFound",
               defaults: new { controller = "Error", action = "PageNotFound", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Error3",
               url: "UnauthorizedForbidden",
               defaults: new { controller = "Error", action = "UnauthorizedForbidden", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "BookRead",
                url: "BookRead/{id}",
                defaults: new { controller = "Book", action = "BookRead", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "BookDisplay",
                url: "BookDisplay/{id}",
                defaults: new { controller = "Book", action = "BookDisplay", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "BookCompleted",
                url: "BookCompleted/{id}",
                defaults: new { controller = "Book", action = "BookCompleted", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "BookReading",
               url: "BookReading/{id}",
               defaults: new { controller = "Book", action = "BookReading", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "LoginPage",
                url: "",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Search",
                url: "Search/{id}",
                defaults: new { controller = "Main", action = "Search", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Profile",
               url: "Profile",
               defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "Announcements",
              url: "Announcements",
              defaults: new { controller = "Main", action = "Announcements", id = UrlParameter.Optional }
          );
            routes.MapRoute(
             name: "Setting",
             url: "Setting",
             defaults: new { controller = "Main", action = "Setting", id = UrlParameter.Optional }
         );
            routes.MapRoute(
            name: "Logout",
            url: "Logout",
            defaults: new { controller = "Login", action = "UserLogout", id = UrlParameter.Optional }
        );
            routes.MapRoute(
          name: "Activity",
          url: "Activity/{id}",
          defaults: new { controller = "Book", action = "Activity", id = UrlParameter.Optional }
      );
            routes.MapRoute(
          name: "SetBookActivity",
          url: "SetBookActivity",
          defaults: new { controller = "Book", action = "SetBookActivity", id = UrlParameter.Optional }
      );

            routes.MapRoute(
                name: "Home",
                url: "Home",
                defaults: new { controller = "Main", action = "Home" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
