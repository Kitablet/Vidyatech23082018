using FISE_Cloud.Caching;
using FISE_Cloud.Models;
using FISE_Cloud.TWebClients;
using System;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;

namespace FISE_Cloud.Services.Authentication
{
    public class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly TimeSpan _expirationTimeSpan;
        private readonly ICacheManager _cacheManager;
        private readonly ITWebClient _userWebClient;
        private User _cachedUser;

        public FormsAuthenticationService()
        {
            this._httpContext = (new HttpContextWrapper(HttpContext.Current) as HttpContextBase);
            this._cacheManager = new CacheManager();
            this._expirationTimeSpan = FormsAuthentication.Timeout;
            this._userWebClient = new TWebClient(0);
        }

        public virtual void SignIn(User _User, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var userCacheKey = string.Format(CacheKey.USER_KEY, _User.UserId);
            var userData = new UserData
            {
                Role = _User.Role,
                UserCacheKey = userCacheKey,
                UserId = _User.UserId,
                UserKey = _User.Username,
                DisplayName=_User.FirstName +" "+_User.LastName,
                SchoolUId=_User.SchoolUId
            };
            var userDataString = JsonConvert.SerializeObject(userData);

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                _User.Username,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                userDataString,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);

            _cachedUser = _User;

            if (_cacheManager.IsSet(userCacheKey))
                _cacheManager.Remove(userCacheKey);
            _cacheManager.Set(userCacheKey, _cachedUser, (int)_expirationTimeSpan.TotalMinutes);
        }

        public virtual void SignOut()
        {
            _cachedUser = null;
            FormsAuthentication.SignOut();
        }
        public virtual bool IsUserAuthenticated
        {
            get
            {
                if (_httpContext == null ||
                   _httpContext.Request == null ||
                   !_httpContext.Request.IsAuthenticated ||
                   !(_httpContext.User.Identity is FormsIdentity))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public virtual User GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null)
            {
                _cachedUser = user;
                var userCacheKey = string.Format(CacheKey.USER_KEY, user.UserId);
                _cacheManager.Remove(userCacheKey);
                _cacheManager.Set(userCacheKey, _cachedUser, (int)_expirationTimeSpan.TotalMinutes);
            }
            return _cachedUser;
        }

        public virtual User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket)
        {           
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var email = ticket.Name;
            if (String.IsNullOrWhiteSpace(email))
                return null;

            User user = null;
            var userDataFromTicket = ticket.UserData;

            try
            {
                UserData userData = JsonConvert.DeserializeObject<UserData>(userDataFromTicket);
            }
            catch {
            }
            
            return user;
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public virtual User CurrentUser
        {
            get
            {
                User user = null;
                if (_httpContext.User.Identity.IsAuthenticated)
                {
                    var userDatatemp = (AuthPrincipal)(HttpContext.Current.User);
                    if (userDatatemp.UserId != 0)
                    {
                        if (_cachedUser != null)
                            return _cachedUser;

                        ////first check User in Cache
                        if (user == null)
                        {
                            var userCacheKey = CurrentUserData.UserCacheKey;
                            user = _cacheManager.Get<User>(userCacheKey);
                        }

                        //get registered user
                        if (user == null)
                        {
                            user = GetAuthenticatedUser();
                        }

                        if (user != null)
                            _cachedUser = user;
                        return _cachedUser;
                    }
                }
                return user;
            }
        }

        public virtual User CachedUser
        {
            get
            {
                return _cachedUser;
            }
        }

        public virtual UserData CurrentUserData
        {
            get
            {
                UserData userData = null;
                if (_httpContext.User.Identity.IsAuthenticated)
                {
                    var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
                    var ticket = formsIdentity.Ticket;
                    try
                    {
                        userData = JsonConvert.DeserializeObject<UserData>(ticket.UserData);
                    }
                    catch
                    {
                        var userDatatemp = (AuthPrincipal)(HttpContext.Current.User);
                        userData = new UserData
                        {
                            Role = userDatatemp.Role,
                            UserCacheKey = userDatatemp.UserCacheKey,
                            SchoolUId = userDatatemp.SchoolUId,
                            UserId = userDatatemp.UserId,
                            UserKey = userDatatemp.UserKey
                        };

                    }
                }
                return userData;
            }
        }
    }

    public class UserData
    {
        public string Role { get; set; }
        public string UserCacheKey { get; set; }
        public int UserId { get; set; }
        public string SchoolUId { get; set; }
        public string UserKey { get; set; }
        public string DisplayName { get; set; }
    }
}