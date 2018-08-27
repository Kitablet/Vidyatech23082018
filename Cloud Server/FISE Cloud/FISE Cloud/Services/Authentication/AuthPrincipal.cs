using System.Security.Principal;
using System.Web.Security;

namespace FISE_Cloud.Services.Authentication
{
    public partial class AuthPrincipal:IPrincipal
    {
        public IIdentity Identity
        {
            get;
            private set;
        }
        public bool IsInRole(string role)
        {
            return role.Contains(Role);
        }
        public AuthPrincipal(FormsAuthenticationTicket ticket)
        {
            //this.Identity = new GenericIdentity(email);
            this.Identity = new FormsIdentity(ticket);
        }
        public string Role { get; set; }
        public string UserCacheKey { get; set; }
        public int UserId { get; set; }
        public string SchoolUId { get; set; }
        public string UserKey { get; set; }
        public string DisplayName { get; set; }

    }
}