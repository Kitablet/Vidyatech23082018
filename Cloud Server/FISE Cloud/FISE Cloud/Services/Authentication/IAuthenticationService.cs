using FISE_Cloud.Models;

namespace FISE_Cloud.Services.Authentication
{
    public partial interface IAuthenticationService
    {
        void SignIn(User User, bool createPersistentCookie);
        void SignOut();
        User GetAuthenticatedUser();
        User CurrentUser { get; }
        UserData CurrentUserData { get; }
        bool IsUserAuthenticated { get; }
    }
}