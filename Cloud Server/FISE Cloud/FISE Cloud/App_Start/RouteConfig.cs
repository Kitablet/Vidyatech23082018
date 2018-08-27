using System.Web.Mvc;
using System.Web.Routing;
using FISE_Cloud;

namespace FISE_Cloud
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "HomePage",
                "",
                new { controller = "Home", action = "Index" },
                new string[] { "FISE_Cloud.Controllers" }
            );

            #region Users Related

            routes.MapRoute(
                "Login",
                "login/",
                new { controller = "User", action = "Login" },
                new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
                "Logout",
                "logout/",
                new { controller = "User", action = "Logout" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
                "ChangePassword",
                "changePassword/",
                new { controller = "User", action = "ChangePassword" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
                "PasswordRecovery",
                "passwordrecovery/",
                new { controller = "User", action = "PasswordRecovery" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
                "PasswordRecoveryConfirm",
                "passwordrecovery/confirm/",
                new { controller = "User", action = "PasswordRecoveryConfirm" },
                new string[] { "FISE_Cloud.Controllers" }
                );
            routes.MapRoute(
                "PasswordRecoveryConfirmStudent",
                "passwordrecovery/student/",
                new { controller = "User", action = "PasswordRecoveryConfirmStudent" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
                "UsernameRecovery",
                "usernamerecovery/",
                new { controller = "User", action = "UsernameRecovery" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
                "UserRegistration",
                "register/{token}",
                new { controller = "User", action = "UserRegistration" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
                "StudentRegistration",
                "registerstudent/{schooluid}/{studentid}",
                new { controller = "Parent", action = "StudentRegistration" },
                new string[] { "FISE_Cloud.Controllers" }
                );
            routes.MapRoute(
               "StudentProfile",
               "studentprofile/{schooluid}/{studentid}",
               new { controller = "User", action = "StudentRegistration" },
               new string[] { "FISE_Cloud.Controllers" }
               );
            routes.MapRoute(
                 "UnauthorizedAccess",
                 "Unauthorized/",
                 new { controller = "Common", action = "UnauthorizedAccess" },
                 new string[] { "FISE_Cloud.Controllers" }
                 );
            routes.MapRoute(
                 "PageNotFound",
                 "pagenotfound/",
                 new { controller = "Common", action = "PageNotFound" },
                 new string[] { "FISE_Cloud.Controllers" }
                 );

            routes.MapRoute(
                "UserProfileInfo",
                "user/profile/",
                new { controller = "User", action = "UserProfileInfo" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
                "EditUserProfileInfo",
                "user/profile/edit",
                new { controller = "User", action = "EditUserProfileInfo" },
                new string[] { "FISE_Cloud.Controllers" }
                );
            #endregion

            #region School Related

            routes.MapRoute(
                "SchoolList",
                "schools/{pagesize}/{pageindex}",
                new { controller = "School", action = "SchoolList", pageindex = UrlParameter.Optional, pagesize = UrlParameter.Optional },
                new { pagesize = @"\d*", pageindex = @"\d*" },
                new string[] { "FISE_Cloud.Controllers" }
                );
            routes.MapRoute(
                "CreateSchool",
                "createschool/",
                new { controller = "School", action = "CreateSchool" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
             "SchoolHomePage",
             "schools/school/{schooluid}/details/{pagesize}/{pageindex}",
             new { controller = "School", action = "SchoolHomePage", pageindex = UrlParameter.Optional, pagesize = UrlParameter.Optional },
             new string[] { "FISE_Cloud.Controllers" }
             );

            routes.MapRoute(
              "DisableSchool",
              "school/disableschool/{schoolid}",
              new { controller = "School", action = "DisableSchool" },
              new string[] { "FISE_Cloud.Controllers" }
              );
            routes.MapRoute(
               "EditSchool",
               "schools/school/edit/{schooluid}",
               new { controller = "School", action = "EditSchool" },
                new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
                "CreateSchoolAdmin",
                "schools/school/{schooluid}/addadmin/",
                new { controller = "School", action = "CreateSchoolAdmin" },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
               "SchoolAdminInfo",
               "schools/school/{schooluid}/schooladmin/{adminid}/details",
               new { controller = "School", action = "SchoolAdminInfo" },
               new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
              "DisableSchoolAdmin",
              "school/disableschooladmin/{adminid}",
              new { controller = "School", action = "DisableSchoolAdmin" },
              new string[] { "FISE_Cloud.Controllers" }
              );

            routes.MapRoute(
               "EditSchoolAdmin",
               "schools/school/{schooluid}/schooladmin/edit/{adminid}/",
               new { controller = "School", action = "EditSchoolAdmin" },
               new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
              "StudentandParentInfo",
              "schools/school/{schooluid}/student/{studentid}/details",
              new { controller = "School", action = "StudentandParentInfo" },
              new string[] { "FISE_Cloud.Controllers" }
              );

            routes.MapRoute(
              "DisableParentStudent",
              "school/disableparentstudent/{userid}",
              new { controller = "School", action = "DisableParentStudent" },
              new string[] { "FISE_Cloud.Controllers" }
              );

            routes.MapRoute(
              "StudentandParentInfoAdmin",
              "school/{schooluid}/student/{studentid}/",
              new { controller = "SchoolAdmin", action = "StudentandParentInfoAdmin" },
              new string[] { "FISE_Cloud.Controllers" }
              );
            routes.MapRoute(
             "SchoolAdmin",
             "admin/{schooluid}/{pagesize}/{pageindex}",
             new { controller = "SchoolAdmin", action = "SchoolAdminDashboard", pageindex = UrlParameter.Optional, pagesize = UrlParameter.Optional },
             new string[] { "FISE_Cloud.Controllers" }
             );


            routes.MapRoute(
             "Search",
             "search/{pagesize}/{pageindex}",
             new { controller = "Search", action = "Search", pageindex = UrlParameter.Optional, pagesize = UrlParameter.Optional },
             new string[] { "FISE_Cloud.Controllers" }
             );

            #endregion

            #region Super Admin Related
            routes.MapRoute(
               "ElibraryAdminInfo",
               "elibraryadmins/admin/{elibadminid}",
               new { controller = "SuperAdmin", action = "ElibraryAdminInfo" },
                new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
                "ElibraryAdminList",
                "elibraryadmins/{pagesize}/{pageindex}",
                new { controller = "SuperAdmin", action = "ElibraryAdminList", pageindex = UrlParameter.Optional, pagesize = UrlParameter.Optional },
                new string[] { "FISE_Cloud.Controllers" }
                );

            routes.MapRoute(
               "EditElibraryAdmin",
               "elibraryadmins/admin/edit/{elibadminid}",
               new { controller = "SuperAdmin", action = "EditElibraryAdmin" },
                new string[] { "FISE_Cloud.Controllers" }
               );
            routes.MapRoute(
              "DisableElibraryAdmin",
              "superadmin/disableelibraryadmin/{elibadminid}",
              new { controller = "SuperAdmin", action = "DisableElibraryAdmin" },
              new string[] { "FISE_Cloud.Controllers" }
              );

            routes.MapRoute(
               "EditStudent",
               "editstudent/edit/{schooluid}/{studentid}",
               new { controller = "School", action = "EditStudent" },
                new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
               "EditParent",
               "editparent/edit/{schooluid}/{studentid}/{userid}",
               new { controller = "School", action = "EditParent" },
                new string[] { "FISE_Cloud.Controllers" }
               );


            routes.MapRoute(
               "CreateElibraryAdmin",
               "createelibraryadmin/",
               new { controller = "SuperAdmin", action = "CreateElibraryAdmin" },
                new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
                "ExportSelectedBookstoExcel",
                "books/exportselectedbookstoexcel",
                new { controller = "Books", action = "ExportSelectedBookstoExcel" },
                new string[] { "FISE_Cloud.Controllers" }
                );
            routes.MapRoute(
               "BookInfo",
               "books/book/{bookid}",
               new { controller = "Books", action = "BookInfo" },
               new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
              "DisableBook",
              "books/disablebook/{bookid}",
              new { controller = "Books", action = "DisableBook" },
              new string[] { "FISE_Cloud.Controllers" }
              );

            routes.MapRoute(
               "BooksList",
               "books/{pagesize}/{pageindex}",
               new { controller = "Books", action = "BooksList", pageindex = UrlParameter.Optional, pagesize = UrlParameter.Optional },
               new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
               "EditBooksMetadata",
               "books/book/edit/{bookid}",
               new { controller = "Books", action = "EditBooksMetadata" },
               new string[] { "FISE_Cloud.Controllers" }
               );

            routes.MapRoute(
            "ReportsListing",
            "Reports/",
            new { controller = "Report", action = "Reports" },
            new string[] { "FISE_Cloud.Controllers" }
            );

            routes.MapRoute(
            "Report1",
            "Report1/",
            new { controller = "Report", action = "Report1" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
            "Report2",
            "Report2/",
            new { controller = "Report", action = "Report2" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
            "Report3",
            "Report3",
            new { controller = "Report", action = "Report3" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
            "Report4",
            "Report4/",
            new { controller = "Report", action = "Report4" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
            "Report5",
            "Report5/",
            new { controller = "Report", action = "Report5" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
            "Report6",
            "Report6/",
            new { controller = "Report", action = "Report6" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
            "Report7",
            "Report7/",
            new { controller = "Report", action = "Report7" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            routes.MapRoute(
            "Report8",
            "Report8/",
            new { controller = "Report", action = "Report8" },
            new string[] { "FISE_Cloud.Controllers" }
            );

            routes.MapRoute(
            "Report1SchoolAdmin",
            "Report1SchoolAdmin/",
            new { controller = "Report", action = "Report1SchoolAdmin" },
            new string[] { "FISE_Cloud.Controllers" }
            );
            #endregion

            routes.MapRoute(
           "ImportStudents",
           "ImportStudents/",
           new { controller = "School", action = "ImportExportStudents" },
           new string[] { "FISE_Cloud.Controllers" }
           );
            routes.MapRoute(
           "ImportChildren",
           "ImportChildren/",
           new { controller = "School", action = "ImportExportChildren" },
           new string[] { "FISE_Cloud.Controllers" }
           );
            routes.MapRoute(
          "BulkUpdateStudents",
          "BulkUpdateStudents/",
          new { controller = "School", action = "BulkUpdate" },
          new string[] { "FISE_Cloud.Controllers" }
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
