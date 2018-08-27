using System.Web.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FISE_API.Services;
using System;
using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using FISE_API.Models;
using Newtonsoft.Json.Converters;
using System.Linq;
namespace FISE_API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Filters.HMACAuthentication]
    [MyCustomActionFilter]
    public class FISEAPIController : ApiController
    {
        private FISEService _FISEService;
        private readonly int _pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
        private readonly string _baseUrl = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"];
        public static int _currentUserId { get; set; }
        public FISEAPIController()
        {
            _FISEService = new FISEService();
        }
        #region Connection
        [HttpGet]
        [ActionName("connectserver")]
        public bool connectServer()
        {
            return true;
        }

        [HttpPost]
        public IHttpActionResult connectServerPost(JObject Response)
        {
            return Ok(Response);
        }
        #endregion

        #region AddNewUser
        [HttpPost]
        [ActionName("adduser")]
        public string AddNewUser(JObject Response)
        {
            UserResult _UserResult = new UserResult();
            string Email = string.Empty;
            string MobileNo = string.Empty;
            string UserType = string.Empty;
            string Url = string.Empty;
            if (Response["MobileNo"] != null && Response["MobileNo"].ToString() != "undefined")
            {
                MobileNo = Response["MobileNo"].ToString();
            }
            if (Response["Email"] != null && Response["Email"].ToString() != "undefined")
            {
                Email = Response["Email"].ToString();
            }
            if (Response["UserType"] != null && Response["UserType"].ToString() != "undefined")
            {
                UserType = Response["UserType"].ToString();
            }
            if (Response["Url"] != null && Response["Url"].ToString() != "undefined")
            {
                Url = Response["Url"].ToString();
            }
            if (string.IsNullOrEmpty(MobileNo) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(UserType))
            {
                return UserStatus.Error.ToString();
            }
            _FISEService._currentUserId = _currentUserId;
            _UserResult = _FISEService.AddNewUser(Email, MobileNo, UserType);
            if (_UserResult.Status == UserStatus.Sucess)
            {
                return UserStatus.Sucess.ToString();
            }
            else
            {
                return UserStatus.Error.ToString();
            }

        }
        #endregion

        #region Registration
        [HttpPost]
        [ActionName("register")]
        public UserStatus PostRegister(JObject Response)
        {
            User guestUser = JsonConvert.DeserializeObject<User>(Response.ToString());
            if (ModelState.IsValid)
            {

                if (String.IsNullOrEmpty(guestUser.Token))
                {
                    return UserStatus.Error;
                }
                if (guestUser.DobDate > 0 && guestUser.DobMonth > 0 && guestUser.DobYear > 0)
                {
                    guestUser.DateOfBirth = new DateTime(guestUser.DobYear, guestUser.DobMonth, guestUser.DobDate);
                }
                _FISEService._currentUserId = _currentUserId;
                int? id = _FISEService.Register(guestUser);
                if (id == 0)
                {
                    return UserStatus.Error;
                }
                else if (id == 1)
                {
                    return UserStatus.Sucess;
                }
                else if (id == 2)
                {
                    return UserStatus.UserAlreadyRegistered;
                }
                else if (id == 3)
                {
                    return UserStatus.InvalidToken;
                }
                else
                {
                    return UserStatus.Error;
                }
            }
            else
            {
                return UserStatus.Error;
            }
        }

        [HttpGet]
        [ActionName("getregisteration")]
        public JObject GetRegisterationDetailsByToken(string Token, int ParentId = 0)
        {
            if (!String.IsNullOrEmpty(Token))
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetRegisteration(Token, ParentId);
            }
            else
            {
                return JObject.FromObject(new UserBase { returnStatus = (int)UserStatus.Error });
            }
        }

        [HttpPost]
        [ActionName("VerifyMobile")]
        public ReturnStatus VerifyMobile(JObject Response)
        {
            ReturnStatus _Result = new ReturnStatus();
            string Email = string.Empty;
            string MobileNo = string.Empty;
            string Type = string.Empty;
            int EntityId = 0;
            if (Response["MobileNo"] != null && Response["MobileNo"].ToString() != "undefined")
            {
                MobileNo = Response["MobileNo"].ToString();
            }
            if (Response["Email"] != null && Response["Email"].ToString() != "undefined")
            {
                Email = Response["Email"].ToString();
            }

            if (Response["Type"] != null && Response["Type"].ToString() != "undefined")
            {
                Type = Response["Type"].ToString();
            }
            if (Response["EntityId"] != null && Response["EntityId"].ToString() != "undefined")
            {
                EntityId = int.Parse(Response["EntityId"].ToString());
            }
            if (string.IsNullOrEmpty(MobileNo) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Type) || EntityId == 0)
            {
                return new ReturnStatus { returnStatus = 0 };
            }
            _FISEService._currentUserId = _currentUserId;
            _Result = _FISEService.VerifyMobile(Email, MobileNo, Type, EntityId);
            return _Result;
        }

        /// <summary>
        /// Register a student
        /// </summary>
        [HttpPost]
        [ActionName("registerstudent")]
        public UserStatus RegisterStudent(JObject Response)
        {
            StudentRegistrationModel _student = JsonConvert.DeserializeObject<StudentRegistrationModel>(Response.ToString());
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.RegisterStudent(_student);
        }
        #endregion

        #region Login
        [HttpPost]
        [ActionName("browserlogin")]
        public HttpResponseMessage BrowserLogin(JObject Response)
        {
            BrowserLoginModel _LoginModel = null;
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                _LoginModel = JsonConvert.DeserializeObject<BrowserLoginModel>(Response.ToString());
                if (!(String.IsNullOrEmpty(_LoginModel.Username) && String.IsNullOrEmpty(_LoginModel.Password)))
                {
                    _FISEService._currentUserId = _currentUserId;
                    string ssd = _FISEService.BrowserValidateLogin(_LoginModel.Username, _LoginModel.Password, "app",_LoginModel.Plateform,_LoginModel.Browser,_LoginModel.SessionId,_LoginModel.IPAddress);
                    response.Content = new StringContent(ssd, Encoding.UTF8, "application/json");
                    return response;
                }
            }
            catch { }
            finally
            {
            }
            response.Content = new StringContent(JsonConvert.SerializeObject(new { Status = UserStatus.Error }), Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage Login(JObject Response)
        {
            LoginModel _LoginModel = null;
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                _LoginModel = JsonConvert.DeserializeObject<LoginModel>(Response.ToString());
                if (!(String.IsNullOrEmpty(_LoginModel.Username) && String.IsNullOrEmpty(_LoginModel.Password)))
                {
                    _FISEService._currentUserId = _currentUserId;
                    string ssd = _FISEService.ValidateLogin(_LoginModel.Username, _LoginModel.Password, "app");
                    response.Content = new StringContent(ssd, Encoding.UTF8, "application/json");
                    return response;
                }
            }
            catch { }
            finally
            {
            }
            response.Content = new StringContent(JsonConvert.SerializeObject(new { Status = UserStatus.Error }), Encoding.UTF8, "application/json");
            return response;
        }
        [HttpPost]
        [ActionName("logincloud")]
        public HttpResponseMessage LoginCloud(JObject Response)
        {
            LoginModel _LoginModel = null;
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                _LoginModel = JsonConvert.DeserializeObject<LoginModel>(Response.ToString());
                if (!(String.IsNullOrEmpty(_LoginModel.Username) && String.IsNullOrEmpty(_LoginModel.Password)))
                {
                    _FISEService._currentUserId = _currentUserId;
                    string ssd = _FISEService.ValidateLogin(_LoginModel.Username, _LoginModel.Password, "cloud");
                    response.Content = new StringContent(ssd, Encoding.UTF8, "application/json");
                    return response;
                }
            }
            catch { }
            finally
            {
            }
            response.Content = new StringContent(JsonConvert.SerializeObject(new { Status = UserStatus.Error }), Encoding.UTF8, "application/json");
            return response;
        }
        [HttpGet]
        [ActionName("getuserbyusername")]
        public JObject GetUserByUserName(string Username)
        {
            JObject _User = null;

            if (!string.IsNullOrEmpty(Username))
            {
                _FISEService._currentUserId = _currentUserId;
                _User = _FISEService.GetUserByUserName(Username);
                if (_User != null)
                {
                    _User["Password"] = "";
                    _User["PasswordSalt"] = "";
                    return JObject.FromObject(new { Status = UserStatus.Sucess, User = _User });
                }
                else
                {

                }
            }
            return JObject.FromObject(new { Status = UserStatus.Error, User = _User });
        }

        [HttpGet]
        [ActionName("getuserbyid")]
        public JObject GetUserById(int UserId)
        {
            JObject _User = null;
            if (UserId > 0)
            {
                _FISEService._currentUserId = _currentUserId;
                _User = _FISEService.GetUserById(UserId);
                if (_User != null)
                {
                    _User["Password"] = "";
                    _User["PasswordSalt"] = "";
                    return JObject.FromObject(new { Status = UserStatus.Sucess, User = _User });
                }
                else
                {

                }
            }
            return JObject.FromObject(new { Status = UserStatus.Error, User = _User });
        }

        [HttpPost]
        [ActionName("validateschooladmin")]
        public UserStatus ValidateSchoolAdmin(JObject Response)
        {
            LoginModel _LoginModel = null;
            try
            {
                _LoginModel = JsonConvert.DeserializeObject<LoginModel>(Response.ToString());
                if (!(String.IsNullOrEmpty(_LoginModel.Username) && String.IsNullOrEmpty(_LoginModel.Password)))
                {
                    _FISEService._currentUserId = _currentUserId;
                    return _FISEService.ValidateSchoolAdmin(_LoginModel.Username, _LoginModel.Password);
                }
            }
            catch { }
            finally
            {
            }
            return UserStatus.Error;
        }
        #endregion

        #region PasswordRecovery
        [HttpPost]
        [ActionName("sendpasswordrecovery")]
        public string SendPasswordRecovery(JObject Response)
        {
            UserResult _UserResult = new UserResult();
            string Username = string.Empty;
            string Url = string.Empty;
            if (Response["Username"] != null && Response["Username"].ToString() != "undefined")
            {
                Username = Response["Username"].ToString();
            }
            if (Response["Url"] != null && Response["Url"].ToString() != "undefined")
            {
                Url = Response["Url"].ToString();
            }
            if (!String.IsNullOrEmpty(Username))
            {
                _FISEService._currentUserId = _currentUserId;
                _UserResult = _FISEService.SendPasswordRecovery(Username, Url);
            }
            return ((int)_UserResult.Status).ToString();
        }

        [HttpPost]
        [ActionName("validateusertoken")]
        public GenericStatus ValidateUserToken(JObject Response)
        {
            string Token = string.Empty;
            string Type = string.Empty;
            try
            {
                if (Response["Token"] != null && Response["Token"].ToString() != "undefined")
                {
                    Token = Response["Token"].ToString();
                }
                if (Response["Type"] != null && Response["Type"].ToString() != "undefined")
                {
                    Type = Response["Type"].ToString();
                }
                if (String.IsNullOrEmpty(Token) || String.IsNullOrEmpty(Type))
                {
                    return GenericStatus.Error;
                }
                else
                {
                    _FISEService._currentUserId = _currentUserId;
                    return _FISEService.ValidateUserToken(Token, Type);
                }
            }
            catch { }
            return GenericStatus.Error;
        }

        [HttpPost]
        [ActionName("passwordrecoverysubmit")]
        public UserStatus PasswordRecoverySubmit(JObject Response)
        {
            string Token = string.Empty;
            string NewPassword = string.Empty;
            try
            {
                if (Response["Token"] != null && Response["Token"].ToString() != "undefined")
                {
                    Token = Response["Token"].ToString();
                }
                if (Response["NewPassword"] != null && Response["NewPassword"].ToString() != "undefined")
                {
                    NewPassword = Response["NewPassword"].ToString();
                }
                if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(NewPassword))
                {
                    return UserStatus.Error;
                }
                else
                {
                    _FISEService._currentUserId = _currentUserId;
                    return _FISEService.PasswordRecoverySubmit(Token, NewPassword);
                }
            }
            catch { }
            return UserStatus.Error;
        }

        [HttpPost]
        [ActionName("sendpasswordrecoveryreader")]
        public UserPasswordRecovery SendPasswordRecoveryReader(JObject Response)
        {
            UserPasswordRecovery _UserResult = new UserPasswordRecovery();
            string Username = string.Empty;
            string Url = string.Empty;
            string Environment = string.Empty;
            if (Response["Username"] != null && Response["Username"].ToString() != "undefined")
            {
                Username = Response["Username"].ToString();
            }
            if (Response["Url"] != null && Response["Url"].ToString() != "undefined")
            {
                Url = Response["Url"].ToString();
            }
            if (Response["Environment"] != null && Response["Environment"].ToString() != "undefined")
            {
                Environment = Response["Environment"].ToString();
            }
            if (!String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Environment))
            {
                _FISEService._currentUserId = _currentUserId;

                _UserResult = _FISEService.SendPasswordRecoveryReader(Username, Environment);
            }
            return _UserResult;
        }

        [HttpPost]
        [ActionName("sendschoolstudentpasswordrecovery")]
        public UserPasswordRecovery SendSchoolStudentPasswordRecovery(JObject Response)
        {
            UserPasswordRecovery _UserResult = new UserPasswordRecovery();
            string UserName = string.Empty;
            int ToId = 0;
            string Environment = string.Empty;
            if (Response["UserName"] != null && Response["UserName"].ToString() != "undefined")
            {
                UserName = Response["UserName"].ToString();
            }
            if (Response["ToId"] != null && Response["ToId"].ToString() != "undefined")
            {
                ToId = Convert.ToInt32(Response["ToId"].ToString());
            }

            if (!string.IsNullOrEmpty(UserName) && ToId != 0)
            {
                _FISEService._currentUserId = _currentUserId;

                _UserResult = _FISEService.SendSchoolStudentPasswordRecovery(UserName, ToId);
            }
            return _UserResult;
        }


        #endregion

        #region ChangePassword
        [HttpPost]
        [ActionName("changepassword")]
        public UserStatus ChangePassword(JObject Response)
        {
            int UserId = 0;
            string NewPassword = string.Empty;
            string OldPassword = string.Empty;

            if (Response["UserId"] != null && Response["UserId"].ToString() != "undefined")
            {
                UserId = int.Parse(Response["UserId"].ToString());
            }
            if (Response["NewPassword"] != null && Response["NewPassword"].ToString() != "undefined")
            {
                NewPassword = Response["NewPassword"].ToString();
            }
            if (Response["OldPassword"] != null && Response["OldPassword"].ToString() != "undefined")
            {
                OldPassword = Response["OldPassword"].ToString();
            }
            if (UserId == 0 || string.IsNullOrEmpty(NewPassword) || String.IsNullOrEmpty(OldPassword)) { return UserStatus.Error; }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.ChangePassword(UserId, NewPassword, OldPassword);
        }
        #endregion

        #region UserProfile
        /// <summary>
        /// Update user details
        /// </summary>
        [HttpPost]
        [ActionName("updateuserprofile")]
        public UserStatus UpdateUserProfile(JObject Response)
        {
            UserProfile _User = JsonConvert.DeserializeObject<UserProfile>(Response.ToString());
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.UpdateUserProfile(_User);
            }
            catch { }
            finally
            {
                _User = null;
            }
            return UserStatus.Error;
        }
        #endregion

        #region School

        /// <summary>
        /// Create new school
        /// </summary>
        [HttpPost]
        [ActionName("createschool")]
        public SchoolStatus CreateSchool(JObject Response)
        {
            School _School = JsonConvert.DeserializeObject<School>(Response.ToString()); ;

            if (String.IsNullOrEmpty(_School.PrincipalEmail) || String.IsNullOrEmpty(_School.SchoolName) || String.IsNullOrEmpty(_School.PrincipalName))
            {
                return SchoolStatus.Error;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.CreateSchool(_School);
        }

        /// <summary>
        /// Get school details with the help of school unique id
        /// </summary>
        [HttpGet]
        [ActionName("getschoolprofiledetails")]
        public SchoolResult GetSchoolProfileDetails(string SchoolUId)
        {
            if (String.IsNullOrEmpty(SchoolUId)) { return new SchoolResult { Status = SchoolStatus.Error }; }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetSchoolProfileDetails(SchoolUId);
        }

        /// <summary>
        /// Get school details with help of school unique id
        /// </summary>
        [HttpGet]
        [ActionName("getschoolbyuid")]
        public SchoolData GetSchoolByUId(string SchoolUId, int PageIndex = 0, int PageSize = 0)
        {
            if (String.IsNullOrEmpty(SchoolUId))
                return null;
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetSchoolByUId(SchoolUId, PageIndex, PageSize);
        }

        /// <summary>
        /// Update an exting school
        /// </summary>
        [HttpPost]
        [ActionName("updateschool")]
        public SchoolStatus UpdateSchool(JObject Response)
        {
            School _School = null;
            try
            {
                _School = JsonConvert.DeserializeObject<School>(Response.ToString());
                if (_School.SchoolId <= 0 || String.IsNullOrEmpty(_School.SchoolName) || String.IsNullOrEmpty(_School.PrincipalName) || String.IsNullOrEmpty(_School.PrincipalEmail))
                {
                    return SchoolStatus.Error;
                }
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.UpdateSchool(_School);
            }
            catch { }
            finally
            {
            }
            return SchoolStatus.Error;
        }

        /// <summary>
        /// Get school admin details
        /// </summary>
        [HttpGet]
        [ActionName("getschooladmin")]
        public StudentModel GetSchoolAdmin(string SchoolUId, int UserId)
        {
            if (String.IsNullOrEmpty(SchoolUId) || UserId < 0)
                return null;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetSchoolAdmin(SchoolUId, UserId);
        }


        /// <summary>
        /// Get list of school admins
        /// </summary>
        [HttpGet]
        [ActionName("getschooladmins")]
        public PagedList<UserCommon> GetSchoolAdmins(string SchoolUId, int PageIndex = 0, int PageSize = 0, string SearchTxt = null)
        {
            if (String.IsNullOrEmpty(SchoolUId))
                return null;
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetSchoolAdmins(SchoolUId, PageIndex, PageSize, SearchTxt);
        }
        
        /// <summary>
        /// Add new school admins
        /// </summary>
        [HttpPost]
        [ActionName("addschooladmins")]
        public UserStatus AddSchoolAdmins(JObject Response)
        {
            UserResult _UserResult = new UserResult();
            string SchoolUId = string.Empty;
            string Email = string.Empty;
            string MobileNo = string.Empty;
            string Url = string.Empty;
            if (Response["SchoolUId"] != null && Response["SchoolUId"].ToString() != "undefined")
            {
                SchoolUId = Response["SchoolUId"].ToString();
            }
            if (Response["MobileNo"] != null && Response["MobileNo"].ToString() != "undefined")
            {
                MobileNo = Response["MobileNo"].ToString();
            }
            if (Response["Email"] != null && Response["Email"].ToString() != "undefined")
            {
                Email = Response["Email"].ToString();
            }
            if (Response["Url"] != null && Response["Url"].ToString() != "undefined")
            {
                Url = Response["Url"].ToString();
            }
            if (string.IsNullOrEmpty(MobileNo) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(SchoolUId) || string.IsNullOrEmpty(Url))
            {
                return UserStatus.Error;
            }
            _FISEService._currentUserId = _currentUserId;
            _UserResult = _FISEService.AddSchoolAdmins(SchoolUId, Email, MobileNo);
            if (_UserResult.Status == UserStatus.Sucess)
            {
                return UserStatus.Sucess;
            }
            else
            {
                return _UserResult.Status;
            }

        }

        /// <summary>
        /// Get list of schools
        /// </summary>
        [HttpGet]
        [ActionName("getschoollist")]
        public PagedList<School> GetSchoolList(int PageIndex = 0, int PageSize = 0, string SearchTxt = null)
        {
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetSchoolList(PageIndex, PageSize, SearchTxt);
        }

        /// <summary>
        /// Get details of a student along with his/her parent details
        /// </summary>
        [HttpGet]
        [ActionName("getstudentbyid")]
        public StudentParentResult GetStudentById(string SchoolUId, int UserId)
        {
            if (String.IsNullOrEmpty(SchoolUId) || UserId == 0)
                return new StudentParentResult() { APIStatus = UserStatus.Error };
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetStudentById(SchoolUId, UserId);
        }
        
        /// <summary>
        /// Disable a school admin by making it Trashed.
        /// </summary>
        /// <returns>True/False</returns>  
        [HttpPost]
        [ActionName("disableschooladmin")]
        public SchoolAdminDisableStatus DisableSchoolAdmin(JObject Response)
        {
            if (Response == null || Response["UserId"] == null)
                return SchoolAdminDisableStatus.Error;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.DisableSchoolAdmin(Convert.ToInt32(Response["UserId"]));
        }
        
        /// <summary>
        /// Disable a school by making it Trashed.
        /// </summary>
        /// <returns>True/False</returns>  
        [HttpPost]
        [ActionName("disableschool")]
        public SchoolDisableStatus DisableSchool(JObject Response)
        {
            if (Response == null || Response["SchoolId"] == null)
                return SchoolDisableStatus.Error;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.DisableSchool(Convert.ToInt32(Response["SchoolId"]));
        }
        #endregion

        #region CheckDevice
        [HttpGet]
        [ActionName("CheckDevice")]
        public string CheckDeviceEnvironmentByDeviceDetails(string DeviceDetails)
        {
            if (!String.IsNullOrEmpty(DeviceDetails))
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.CheckDevice(DeviceDetails);
            }
            else
            {
                return null;
            }
        }

        #endregion
        #region GetAvatar
        [HttpGet]
        [ActionName("GetAvatar")]
        public List<Avatar> GetAvatarDetailsByUserId(int UserId)
        {
            if (UserId <= 0)
            {

                return null;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetAvatar(UserId);


        }

        #endregion
        #region GetEvents
        [HttpGet]
        [ActionName("GetEvents")]
        public List<UserEvent> GetEventsDetailsByUserId(int UserId)
        {
            if (UserId <= 0)
                return null;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetEvents(UserId);


        }
        [HttpPost]
        [ActionName("UpdateViewEvents")]
        public GenericStatus UpdateViewEvents(JObject Response)
        {
            string EventIds = "";
            int UserId = 0;
            try
            {
                if (Response["EventIds"] != null && Response["EventIds"].ToString() != "undefined")
                {
                    EventIds = Response["EventIds"].ToString();
                }
                if (Response["UserId"] != null && Response["UserId"].ToString() != "undefined")
                {
                    UserId = int.Parse(Response["UserId"].ToString());
                }

                if (UserId > 0 && !String.IsNullOrEmpty(EventIds))
                {
                    _FISEService._currentUserId = _currentUserId;
                    return _FISEService.UpdateViewEvents(EventIds, UserId);
                }
            }
            catch { }
            finally
            {
            }
            return GenericStatus.Error;
        }
        #endregion
        #region AddDeviceInfo
        [HttpPost]
        [ActionName("AddDeviceInfo")]
        public AddDeviceModel AddDeviceInfo(JObject Response)
        {
            DeviceDetail _Device = JsonConvert.DeserializeObject<DeviceDetail>(Response.ToString());

            if (String.IsNullOrEmpty(_Device.DeviceDetails) || String.IsNullOrEmpty(_Device.DeviceName) || String.IsNullOrEmpty(_Device.DeviceOS) || String.IsNullOrEmpty(_Device.Environment))
            {
                return new AddDeviceModel() { Status = DeviceStatus.Error };
            }
            else
            {
                if (_Device.Environment.ToLower() == "home" && _Device.UserId <= 0)
                {
                    return new AddDeviceModel() { Status = DeviceStatus.Error };
                }
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.AddDevice(_Device);
        }
        #endregion

        #region AddEditAvatar
        [HttpPost]
        [ActionName("AddEditAvatar")]
        public bool AddEditAvatarByUserId(JObject Response)
        {
            AddEditAvatar _Avatar = JsonConvert.DeserializeObject<AddEditAvatar>(Response.ToString()); ;

            if ((_Avatar.UserId <= 0) || (_Avatar.AvatarId <= 0))
                return false;

            _FISEService._currentUserId = _currentUserId;
            return _FISEService.AddEditAvatar(_Avatar);
        }
        #endregion
        #region SaveRating
        [HttpPost]
        [ActionName("SaveRating")]
        public bool InsertUpdateBookRatingByUserId(JObject Response)
        {

            BookRating _Rating = JsonConvert.DeserializeObject<BookRating>(Response.ToString()); ;

            if ((_Rating.UserId <= 0) || (_Rating.BookId <= 0))
                return false;

            _FISEService._currentUserId = _currentUserId;
            return _FISEService.SaveRating(_Rating);
        }
        #endregion

        [HttpGet]
        [ActionName("getbookscatlog")]
        public string GetBooksCatlog()
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetBooksCatlog();
        }

        #region Student
        [HttpPost]
        [ActionName("removeusersfromschool")]
        public GenericStatus RemoveUsersFromSchool(JObject Response)
        {
            string UserIds = "";
            int SchoolId = 0;
            try
            {
                if (Response["UserIds"] != null && Response["UserIds"].ToString() != "undefined")
                {
                    UserIds = Response["UserIds"].ToString();
                }
                if (Response["SchoolId"] != null && Response["SchoolId"].ToString() != "undefined")
                {
                    SchoolId = int.Parse(Response["SchoolId"].ToString());
                }

                if (SchoolId > 0 && !String.IsNullOrEmpty(UserIds))
                {
                    _FISEService._currentUserId = _currentUserId;
                    return _FISEService.RemoveUsersFromSchool(UserIds, SchoolId);
                }
            }
            catch { }
            finally
            {
            }
            return GenericStatus.Error;
        }

        [HttpGet]
        [ActionName("getstudentsofschool")]
        public PagedList<StudentModel> GetStudentsOfSchool(string SchoolUId, string Grade, int PageSize = 0, int PageIndex = 1, string SearchTxt = null)
        {
            if (String.IsNullOrEmpty(SchoolUId)) { return null; }
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetStudentsOfSchool(SchoolUId, PageSize, PageIndex, SearchTxt, Grade);
        }

        [HttpGet]
        [ActionName("getstudentofschool")]
        public StudentResult GetStudentOfSchool(int SchoolId, int UserId)
        {
            StudentResult _Result = new StudentResult { Status = UserStatus.Error, Student = null };
            if (SchoolId <= 0 || UserId <= 0) { return null; }
            _FISEService._currentUserId = _currentUserId;
            _Result = _FISEService.GetStudentOfSchool(SchoolId, UserId);

            return _Result;
        }

        /// <summary>
        /// Add new students in a school
        /// </summary>
        [HttpPost]
        [ActionName("importstudents")]
        public StudentImportExportInput ImportStudents(JObject Response)
        {
            _FISEService._currentUserId = _currentUserId;
            StudentImportExportInput Student = Newtonsoft.Json.JsonConvert.DeserializeObject<StudentImportExportInput>(Response.ToString());
            StudentImportExportInput _InsertStatus = _FISEService.ImportStudents(Student.Students, Student.SchoolUId);
            return _InsertStatus;
        }

        [HttpPost]
        [ActionName("importchildren")]
        public ChildImportExportInput ImportChildren(JObject Response)
        {
            _FISEService._currentUserId = _currentUserId;
            ChildImportExportInput Student = Newtonsoft.Json.JsonConvert.DeserializeObject<ChildImportExportInput>(Response.ToString());
            ChildImportExportInput _InsertStatus = _FISEService.ImportChildren(Student.Students, Student.SchoolUId);
            return _InsertStatus;
        }
        [HttpGet]
        [ActionName("readbook")]
        public ReadBookStatus ReadBook(int UserId, string DeviceDetails, int MaxCopies, int BookId)
        {
            if (UserId == 0)
                return ReadBookStatus.Error;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.ReadBook(UserId, DeviceDetails, MaxCopies, BookId);
        }

        [HttpPost]
        [ActionName("readlater")]
        public GenericStatus ReadLater(JObject Response)
        {
            if (Response == null || Response["UserId"] == null || Response["BookId"] == null)
                return GenericStatus.Error;
            try
            {
                int userId = Convert.ToInt32(Response["UserId"]);
                int bookId = Convert.ToInt32(Response["BookId"]);

                _FISEService._currentUserId = _currentUserId;
                return _FISEService.ReadLater(userId, bookId);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        [HttpPost]
        [ActionName("validateimportstudents")]
        public List<StudentImportExport> ValiadteImportStudents(List<StudentImportExport> Students)
        {
            _FISEService._currentUserId = _currentUserId;
            List<StudentImportExport> _InsertStatus = _FISEService.ValidateImportStudents(Students);
            return _InsertStatus;
        }
        [HttpPost]
        [ActionName("validateimportchildren")]
        public List<ChildImport> ValiadteImportChildren(List<ChildImport> Students)
        {
            _FISEService._currentUserId = _currentUserId;
            List<ChildImport> _InsertStatus = _FISEService.ValidateImportChildren(Students);
            return _InsertStatus;
        }

        /// <summary>
        /// Update Student details
        /// </summary>
        [HttpPost]
        [ActionName("updatestudent")]
        public UserStatus UpdateStudent(StudentProfileResult _Student)
        {
            try
            {
                if (_Student.DobYear > 0 && _Student.DobMonth > 0 && _Student.DobDate > 0)
                    _Student.DateOfBirth = new DateTime(_Student.DobYear, _Student.DobMonth, _Student.DobDate);
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.UpdateStudent(_Student);
            }
            catch { }
            finally
            {
                _Student = null;
            }
            return UserStatus.Error;
        }


        #endregion

        #region Elibrary
        [HttpGet]
        [ActionName("getelibraryadminslist")]
        public PagedList<User> GetELibraryAdminsList(int PageIndex = 0, int PageSize = 0, string SearchTxt = null)
        {
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetELibraryAdminsList(PageIndex, PageSize, SearchTxt);
        }

        [HttpPost]
        [ActionName("disableelibadmin")]
        public ElibAdminDisableStatus DisableELibAdmin(JObject Respons)
        {
            string ELibAdminIds = string.Empty;
            if (Respons == null || Respons["UserId"] == null)
                return ElibAdminDisableStatus.Error;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.DisableELibAdmin(Convert.ToInt32(Respons["UserId"]));
        }

        [HttpPost]
        [ActionName("addelibadmin")]
        public UserStatus AddElibAdmin(JObject Response)
        {
            UserResult _UserResult = new UserResult();
            string Email = string.Empty;
            string MobileNo = string.Empty;
            if (Response["MobileNo"] != null && Response["MobileNo"].ToString() != "undefined")
            {
                MobileNo = Response["MobileNo"].ToString();
            }
            if (Response["Email"] != null && Response["Email"].ToString() != "undefined")
            {
                Email = Response["Email"].ToString();
            }
            if (string.IsNullOrEmpty(MobileNo) || string.IsNullOrEmpty(Email))
            {
                return UserStatus.Error;
            }
            _FISEService._currentUserId = _currentUserId;
            _UserResult = _FISEService.AddElibAdmin(Email, MobileNo);
            if (_UserResult.Status == UserStatus.Sucess)
            {
                return UserStatus.Sucess;
            }
            else
            {
                return _UserResult.Status;
            }

        }

        [HttpGet]
        [ActionName("getelibraryadmin")]
        public User GetELibraryAdmin(int UserId)
        {
            if (UserId < 0)
                return null;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetELibraryAdmin(UserId);
        }
        #endregion

        #region "Books"

        /// <summary>
        /// This is to render the book list page
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [ActionName("getbookslistwithfilter")]
        public BooksListResult GetBooksListWithFilter(int PageIndex = 0, int PageSize = 0)
        {
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetBooksListWithFilter(PageIndex, PageSize);

        }

        /// <summary>
        /// This is to handle the actions like paging and filtering posted from the book list page
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [ActionName("getbookslistbyfilter")]
        public PagedList<Book> GetBooksListbyFilter(int PageIndex = 0, int PageSize = 0, string SearchTxt = "", string SubSection = "", string Language = "", string BookType = "",
                                                    bool HasAnimation = false, bool HasReadAloud = false, bool HasActivity = false)
        {
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetBooksListByFilter(PageIndex, PageSize, SearchTxt, SubSection, Language, BookType, HasAnimation, HasReadAloud, HasActivity);

        }
        /// <summary>
        /// Exports the books in an excel file.
        /// </summary>
        /// <returns>List of books</returns>
        /// 
        [HttpGet]
        [ActionName("getbookslistforexport")]
        public List<ExportBook> GetBooksListForExport(string SubSection = "", string Language = "", string BookType = "",
                                                    bool HasAnimation = false, bool HasReadAloud = false, bool HasActivity = false)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetBooksListForExport(SubSection, Language, BookType, HasAnimation, HasReadAloud, HasActivity);

        }
        /// <summary>
        /// Disable a Book by making it Trashed.
        /// </summary>
        /// <returns>True/False</returns>
        [HttpPost]
        [ActionName("disablebook")]
        public BookDisableStatus DisableBook(JObject Response)
        {
            if (Response == null || Response["BookId"] == null)
                return BookDisableStatus.Error;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.DisableBook(Convert.ToInt32(Response["BookId"]));
        }

        /// <summary>
        /// Get Book details with the help of BookId
        /// </summary>
        /// <returns>Book details and metadata</returns>
        [HttpGet]
        [ActionName("getbookdetailsbyid")]
        public BooksDetailsResult GetBookDetailsById(int BookId)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetBookDetailsById(BookId);
        }

        /// <summary>
        /// Update Book details
        /// </summary>
        /// <returns>Book details and metadata</returns>
        [HttpPost]
        [ActionName("updatebookmetadata")]
        public GenericStatus UpdateBooKMetaData(JObject Response)
        {
            Book _book = JsonConvert.DeserializeObject<Book>(Response.ToString());
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.UpdateBooKMetaData(_book);
        }

        [HttpPost]
        [ActionName("releaseuserbooks")]
        public bool ReleaseUserBooks(JObject Response)
        {
            if (Response == null || Response["UserId"] == null || Response["BookIds"] == null || Response["DeviceId"] == null)
                return false;
            try
            {
                string BookIds = Response["BookIds"].ToString();
                int UserId = Convert.ToInt32(Response["UserId"]);
                int DeviceId = Convert.ToInt32(Response["DeviceId"].ToString());
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.ReleaseUserBooks(UserId, BookIds, DeviceId);
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        [ActionName("releaseuserdevices")]
        public bool ReleaseUserDevices(JObject Response)
        {
            if (Response == null || Response["UserId"] == null || Response["DeviceIds"] == null)
                return false;
            try
            {
                string DeviceIds = Response["DeviceIds"].ToString();
                int UserId = Convert.ToInt32(Response["UserId"]);
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.ReleaseUserDevices(UserId, DeviceIds);
            }
            catch
            {
                return false;
            }
        }


        [HttpPost]
        [ActionName("userdownloadbook")]
        public GenericStatus UserDownloadBook(JObject Response)
        {
            int UserId = 0;
            int BookId = 0;
            int DeviceId = 0;
            try
            {
                if (Response["DeviceId"] != null && Response["DeviceId"].ToString() != "undefined")
                {
                    DeviceId = int.Parse(Response["DeviceId"].ToString());
                }
                if (Response["UserId"] != null && Response["UserId"].ToString() != "undefined")
                {
                    UserId = int.Parse(Response["UserId"].ToString());
                }
                if (Response["BookId"] != null && Response["BookId"].ToString() != "undefined")
                {
                    BookId = int.Parse(Response["BookId"].ToString());
                }
                if (UserId > 0 && BookId > 0 && DeviceId > 0)
                {
                    _FISEService._currentUserId = _currentUserId;
                    return _FISEService.UserDownloadBook(UserId, BookId, DeviceId);
                }
            }
            catch { }
            finally
            {
            }
            return GenericStatus.Error;
        }
        #endregion

        #region "Common User"
        [HttpGet]
        [ActionName("IsUsernameUnique")]
        public bool IsUsernameUnique(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.IsUsernameUnique(userName);

        }
        /// <summary>
        /// Disable a Parent or student by making them Trashed.
        /// </summary>
        /// <returns>True/False</returns> 
        [HttpPost]
        [ActionName("disableparentstudent")]
        public ParentStudentDisableStatus DisableParentStudent(JObject Response)
        {
            if (Response == null || Response["UserId"] == null)
                return ParentStudentDisableStatus.Error;
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.DisableParentStudent(Convert.ToInt32(Response["UserId"]));

        }
        [HttpGet]
        [ActionName("search")]
        public SearchResult Search(string Role, int PageIndex, int PageSize, string SearchTxt, int UserId)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.Search(Role, PageIndex, PageSize, SearchTxt, UserId);
        }

        [HttpGet]
        [ActionName("searchstudent")]
        public PagedList<StudentModel> SearchStudent(int PageIndex, int PageSize, string SearchTxt, int UserId)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.SearchStudent(PageIndex, PageSize, SearchTxt, UserId);
        }

        [HttpGet]
        [ActionName("searchschooladmin")]
        public PagedList<StudentModel> SearchSchoolAdmin(int PageIndex, int PageSize, string SearchTxt)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.SearchSchoolAdmin(PageIndex, PageSize, SearchTxt);
        }

        [HttpGet]
        [ActionName("searchschoollist")]
        public PagedList<School> SearchSchoolList(int PageIndex = 0, int PageSize = 0, string SearchTxt = null)
        {
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.SearchSchoolList(PageIndex, PageSize, SearchTxt);
        }

        [HttpGet]
        [ActionName("searchbookslist")]
        public PagedList<Book> SearchBooksList(int PageIndex = 0, int PageSize = 0, string SearchTxt = "")
        {
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.SearchBooksList(PageIndex, PageSize, SearchTxt);

        }
        #endregion

        #region Windows Azure SAS URI
        public string GetBookDownloadUrl(int BookId)
        {
            string _Result = null;
            if (BookId <= 0) { return null; }
            _FISEService._currentUserId = _currentUserId;
            _Result = _FISEService.GetBookDownloadUrl(BookId);

            return _Result;
        }
        #endregion

        #region "Dashboard"
        /// <summary>
        /// Get required data for dashboard of a parent.
        /// </summary>
        [HttpGet]
        [ActionName("getparentdashbord")]
        public ParentDashboardResult GetParentDashbord(int UserId)
        {
            if (UserId <= 0)
            {
                return null;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetParentDashbord(UserId);
        }

        [HttpGet]
        [ActionName("getelibsuperadmindashboard")]
        public ElibSuperAdminDashboardResult GetElibSuperAdminDashboard()
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetElibSuperAdminDashboard();
        }

        [HttpGet]
        [ActionName("getschooladmindashboard")]
        public SchoolData GetSchoolAdminDashboard(int UserId, int PageIndex = 0, int PageSize = 0, string SchoolUId = "")
        {
            if (UserId <= 0)
                return null;
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetSchoolAdminDashboard(UserId, PageIndex, PageSize, SchoolUId);
        }

        [HttpGet]
        [ActionName("getstudentsofschooladmin")]
        public PagedList<StudentModel> GetStudentsOfSchoolAdmin(string SchoolUId, string Grade, int PageSize = 0, int PageIndex = 1)
        {
            if (String.IsNullOrEmpty(SchoolUId)) { return null; }
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetStudentsOfSchoolAdmin(SchoolUId, PageSize, PageIndex, Grade);
        }
        #endregion

        [HttpPost]
        [ActionName("usernamerecovery")]
        public UsernameRecoveryStatus UserNameRecovery(JObject Response)
        {
            if (Response == null || Response["Email"] == null || string.IsNullOrEmpty(Response["Email"].ToString()))
            {
                return UsernameRecoveryStatus.Error;

            }
            else
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.UserNameRecovery(Response["Email"].ToString());
            }
        }

        /// <summary>
        /// Resend the email varification email to school principle
        /// </summary>
        [HttpPost]
        [ActionName("schoolregistrationemail")]
        public SchoolRegistrationEmailStatus SchoolRegistrationEmail(JObject Response)
        {
            try
            {
                if (Response == null || Response["SchoolId"] == null || Convert.ToInt32(Response["SchoolId"].ToString()) == 0)
                    return SchoolRegistrationEmailStatus.Error;
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.SchoolRegistrationEmail(Convert.ToInt32(Response["SchoolId"].ToString()));
            }
            catch
            {
                return SchoolRegistrationEmailStatus.Error;
            }
        }

        /// <summary>
        /// Resend the registration email to a parent
        /// </summary>
        [HttpPost]
        [ActionName("userregistrationemail")]
        public UserStatus UserRegistrationEmail(JObject Response)
        {
            try
            {
                if (Response == null || Response["UserId"] == null || Convert.ToInt32(Response["UserId"].ToString()) == 0)
                    return UserStatus.Error;
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.UserRegistrationEmail(Convert.ToInt32(Response["UserId"].ToString()));
            }
            catch
            {
                return UserStatus.Error;
            }
        }
        /// <summary>
        /// Get the details of a student profile
        /// </summary>
        [HttpGet]
        [ActionName("getstudentprofile")]
        public StudentProfileResult GetStudentProfile(int UserId, string SchoolUId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetStudentProfile(UserId, SchoolUId);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the details of a parent profile
        /// </summary>
        [HttpGet]
        [ActionName("getparentprofile")]
        public JObject GetParentProfile(int UserId, string SchoolUId)
        {
            JObject _User = null;
            try
            {
                _FISEService._currentUserId = _currentUserId;
                _User = _FISEService.GetParentProfile(UserId, SchoolUId);
                if (_User != null)
                {
                    _User["Password"] = "";
                    _User["PasswordSalt"] = "";
                    return JObject.FromObject(new { Status = UserStatus.Sucess, User = _User });
                }
            }
            catch
            {
                return JObject.FromObject(new { Status = UserStatus.Error, User = _User });
            }
            return JObject.FromObject(new { Status = UserStatus.Error, User = _User });
        }

        [HttpPost]
        [ActionName("createhelpitem")]
        public GenericStatus CreateHelpItem(JObject Response)
        {
            string UserMessage = "";
            int UserId = 0;
            try
            {
                if (Response["UserMessage"] != null && Response["UserMessage"].ToString() != "undefined")
                {
                    UserMessage = Response["UserMessage"].ToString();
                }
                if (Response["UserId"] != null && Response["UserId"].ToString() != "undefined")
                {
                    UserId = int.Parse(Response["UserId"].ToString());
                }
                if (UserId > 0 && !String.IsNullOrEmpty(UserMessage))
                {
                    _FISEService._currentUserId = _currentUserId;
                    return _FISEService.CreateHelpItem(UserMessage, UserId);
                }
            }
            catch { }
            finally
            {
            }
            return GenericStatus.Error;
        }
        #region "Reports"
        [HttpGet]
        [ActionName("getregistrationandloginreport")]
        public RegistrationAndLogin GetRegistrationAndLoginReport(string UserType, string SchoolIds, int PageSize, int PageIndex)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetRegistrationAndLoginReport(UserType, SchoolIds, PageSize, PageIndex);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getregistrationandloginreportlist")]
        public PagedList<StudentModel> GetRegistrationAndLoginReportList(string UserType, string SchoolIds, int PageSize, int PageIndex, string Type)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetRegistrationAndLoginReportList(UserType, SchoolIds, PageSize, PageIndex, Type);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getschools")]
        public List<School> GetSchools(int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetSchools(UserId);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport6filter")]
        public Report6 GetReport6Filter()
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetReport6Filter();
        }

        [HttpGet]
        [ActionName("getreport6")]
        public Report6 GetReport6(string SubsectionIds, string LanguageIds)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetReport6(SubsectionIds, LanguageIds);
        }

        [HttpGet]
        [ActionName("getreport6forexport")]
        public List<Book> GetReport6ForExport(string SubsectionIds, string LanguageIds)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetReport6ForExport(SubsectionIds, LanguageIds);
        }

        [HttpGet]
        [ActionName("getreport3fillters")]
        public Report3FilterModel GetReport3Fillters(int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport3Fillters(UserId);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport4fillters")]
        public Report4FilterModel GetReport4Fillters()
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport4Fillters();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validate the detials of student before updating
        /// </summary>
        [HttpPost]
        [ActionName("validatebulkupdatestudents")]
        public List<StudentImportExport> ValidateBulkUpdateStudents(JObject Response)
        {
            List<StudentImportExport> Students = JsonConvert.DeserializeObject<List<StudentImportExport>>(Response["Student"].ToString());
            string SchoolUId = Response["SchoolUId"].ToString();
            _FISEService._currentUserId = _currentUserId;
            List<StudentImportExport> _students = _FISEService.ValidateBulkUpdateStudents(Students, SchoolUId);
            return _students;
        }

        /// <summary>
        /// Update details of students of a school
        /// </summary>
        [HttpPost]
        [ActionName("updatemultiplestudents")]
        public StudentImportExportInput UpdateMultipleStudents(JObject Response)
        {
            _FISEService._currentUserId = _currentUserId;
            StudentImportExportInput Student = Newtonsoft.Json.JsonConvert.DeserializeObject<StudentImportExportInput>(Response.ToString());
            StudentImportExportInput _InsertStatus = _FISEService.UpdateMultipleStudents(Student.Students, Student.SchoolUId);
            return _InsertStatus;
        }

        /// <summary>
        /// Get list of student of a school for excel export
        /// </summary>
        [HttpGet]
        [ActionName("getstudentsofschoolforexport")]
        public List<StudentImportExport> GetStudentsOfSchoolForExport(string SchoolUId, string Grade, int PageSize = 0, int PageIndex = 1, string SearchTxt = null)
        {
            if (String.IsNullOrEmpty(SchoolUId)) { return null; }
            if (PageSize <= 0)
            {
                PageSize = _pageSize;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetStudentsOfSchoolForExport(SchoolUId, PageSize, PageIndex, SearchTxt, Grade);
        }

        [HttpGet]
        [ActionName("getreport7fillters")]
        public Report7FilterModel GetReport7Fillters()
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport7Fillters();
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport7")]
        public Report7Model GetReport7(DateTime StartDate, DateTime EndDate, string SchoolIds, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport7(StartDate, EndDate, SchoolIds, City);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport7export")]
        public List<Report7ExportModel> GetReport7Export(DateTime StartDate, DateTime EndDate, string SchoolIds, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport7Export(StartDate, EndDate, SchoolIds, City);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport2fillters")]
        public Report2FilterModel GetReport2Fillters(int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport2Fillters(UserId);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport3")]
        public Report3 GetReport3(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string BookIds, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport3(StartDate, EndDate, LanguageIds, SubsectionIds, BookIds, City);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport4")]
        public Report4 GetReport4(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string Publisher, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport4(StartDate, EndDate, LanguageIds, SubsectionIds, Publisher, City);
            }
            catch
            {
                return null;
            }
        }


        [HttpGet]
        [ActionName("getreport8fillters")]
        public List<StudentModel> GetReport8Fillters(int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport8Fillters(UserId);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport8")]
        public List<Report8> GetReport8(DateTime StartDate, DateTime EndDate, string StudentIds, int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport8(StartDate, EndDate, StudentIds, UserId);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport8export")]
        public List<Report8Export> GetReport8Export(DateTime StartDate, DateTime EndDate, string StudentIds, int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport8Export(StartDate, EndDate, StudentIds, UserId);
            }
            catch
            {
                return null;
            }
        }


        [HttpGet]
        [ActionName("getreport2")]
        public Report2 GetReport2(DateTime StartDate, DateTime EndDate, string SchoolIds, string GradeIds, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport2(StartDate, EndDate, SchoolIds, GradeIds, City);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport2studentwise")]
        public Report2StudentWise GetReport2StudentWise(DateTime StartDate, DateTime EndDate, string Section, int UserId, int GradeId, int SchoolId,string CallFrom)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport2StudentWise(StartDate, EndDate, Section, UserId, GradeId, SchoolId, CallFrom);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport2export")]
        public List<Report2Export> GetReport2Export(DateTime StartDate, DateTime EndDate, string SchoolIds, string GradeIds, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport2Export(StartDate, EndDate, SchoolIds, GradeIds, City);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport5fillters")]
        public Report5FilterModel GetReport5Fillters(int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport5Fillters(UserId);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport5")]
        public Report5 GetReport5(DateTime StartDate, DateTime EndDate, int SchoolId, string GradeIds)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport5(StartDate, EndDate, SchoolId, GradeIds);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport5export")]
        public List<Report5Export> GetReport5Export(DateTime StartDate, DateTime EndDate, int SchoolId, string GradeIds)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport5Export(StartDate, EndDate, SchoolId, GradeIds);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport3export")]
        public List<Report3Export> GetReport3Export(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string BookIds, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport3Export(StartDate, EndDate, LanguageIds, SubsectionIds, BookIds, City);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport4export")]
        public List<Report3Export> GetReport4Export(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string Publisher, string City)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport4Export(StartDate, EndDate, LanguageIds, SubsectionIds, Publisher, City);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport1schooladminlist")]
        public PagedList<StudentModel> GetReport1SchoolAdminList(string SchoolUId, int PageSize, int PageIndex, string Type, int GradeId, string Section)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport1SchoolAdminList(SchoolUId, PageSize, PageIndex, Type, GradeId, Section);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [ActionName("getreport1schooladmin")]
        public Report1SchoolAdmin GetReport1SchoolAdmin(string SchoolUId, int PageSize, int PageIndex, bool IsExport = false)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetReport1SchoolAdmin(SchoolUId, PageSize, PageIndex, IsExport);
            }
            catch
            {
                return null;
            }
        }
        #endregion
        [HttpGet]
        [ActionName("getusernamebytoken")]
        public string GetUsernameByToken(string token)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetUsernameByToken(token);
            }
            catch
            {
                return null;
            }
        }

        #region User Data Sync
        #region Online Data
        [HttpPost]
        [ActionName("syncuserbrowsingtime")]
        public UserSyncDetails SyncUserBrowsingTime(JObject Response)
        {
            return SyncData(Response.ToString(), "BrowsingData");
        }
        [HttpPost]
        [ActionName("syncbookdata")]
        public UserSyncDetails SyncBookData(JObject Response)
        {
            return SyncData(Response.ToString(), "BookData");
        }
        #endregion
        #region Offline Data
        [HttpPost]
        [ActionName("syncuserdata")]
        public UserSyncDetails SyncUserData(JObject Response)
        {
            return SyncData(Response.ToString(), "UserData");
        }

        [NonAction]
        public UserSyncDetails SyncData(string UserSyncDataString, string Type)
        {
            _FISEService._currentUserId = _currentUserId;
            UserSyncDetails _UserSyncDetails = null;
            try
            {
                UserProgressForSync UserSyncData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProgressForSync>(UserSyncDataString, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd hh:mm:ss tt" });
                if (UserSyncData != null && UserSyncData.UserId > 0 && UserSyncData.DeviceId > 0)
                {
                    _UserSyncDetails = new UserSyncDetails();
                    if ((UserSyncData.BrowsingProgress != null && UserSyncData.BrowsingProgress.Progress.Count > 0) || (UserSyncData.UserProgressBooks != null && UserSyncData.UserProgressBooks.UserProgressBook.Count > 0))
                    {

                        int dataId = _FISEService.InsertSyncData(UserSyncData.UserId, UserSyncDataString, Type);
                        if (dataId > 0)
                        {
                            if (_FISEService.SyncData(UserSyncData, Type, dataId))
                            {
                                _UserSyncDetails.IsSynced = true;
                            }
                        }

                    }
                    _UserSyncDetails.UserDetails = _FISEService.GetUserBooksDetails(UserSyncData.UserId);
                }
            }
            catch (Exception ex)
            {
                _UserSyncDetails = null;
            }
            return _UserSyncDetails;
        }
        #endregion
        #endregion

        /// <summary>
        ///Update user details which is not yet registered
        /// </summary>
        [HttpPost]
        [ActionName("updatecreateduser")]
        public UserStatus UpdateCreatedUser(JObject Response)
        {
            UserResult _UserResult = new UserResult();
            string Email = string.Empty;
            string MobileNo = string.Empty;
            string UserType = string.Empty;
            int UserId = 0;

            if (Response["MobileNo"] != null && Response["MobileNo"].ToString() != "undefined")
            {
                MobileNo = Response["MobileNo"].ToString();
            }
            if (Response["Email"] != null && Response["Email"].ToString() != "undefined")
            {
                Email = Response["Email"].ToString();
            }
            if (Response["UserType"] != null && Response["UserType"].ToString() != "undefined")
            {
                UserType = Response["UserType"].ToString();
            }
            if (Response["UserId"] != null && Response["UserId"].ToString() != "undefined")
            {
                UserId = Convert.ToInt32(Response["UserId"].ToString());
            }
            if (string.IsNullOrEmpty(MobileNo) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(UserType) || UserId == 0)
            {
                return UserStatus.Error;
            }
            _FISEService._currentUserId = _currentUserId;
            _UserResult = _FISEService.UpdateCreatedUser(Email, MobileNo, UserId, UserType);
            if (_UserResult.Status == UserStatus.Sucess)
            {
                return UserStatus.Sucess;
            }
            else
            {
                return _UserResult.Status;
            }

        }

        [HttpGet]
        [ActionName("getlastemailsenddate")]
        public string GetLastemailSendDate(int UserId, string Type)
        {
            try
            {
                return _FISEService.GetLastemailSendDate(UserId, Type);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region"Browser APIS"

        [HttpGet]
        [ActionName("getuserdata")]
        public string GetUserData(int UserId)
        {
            return _FISEService.GetUserBooksDetails(UserId);
        }

        [HttpGet]
        [ActionName("getbookdetail")]
        public BookRead GetBookDetail(int UserId, int BookId)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.GetBookDetail(UserId, BookId);
        }

        [HttpGet]
        [ActionName("bookdisplay")]
        public BookDisplay BookDisplay(int UserId, int BookId)
        {

            _FISEService._currentUserId = _currentUserId;
            return _FISEService.BookDisplay(UserId, BookId);
        }

        [HttpPost]
        [ActionName("setbookreading")]
        public GenericStatus SetBookReading(JObject Response)
        {
            try
            {
                if (Response["UserId"] == null || Response["BookId"] == null || Response["IsCompleted"] == null || Response["CurrentPage"] == null || Response["page"] == null || Response["Environment"] == null || Response["Platform"] == null)
                    return GenericStatus.Error;
                int UserId = Convert.ToInt32(Response["UserId"]);
                int BookId = Convert.ToInt32(Response["BookId"]);
                bool IsCompleted = Convert.ToBoolean(Response["IsCompleted"]);
                int CurrentPage = Convert.ToInt32(Response["CurrentPage"]);
                string Environment = Response["Environment"].ToString();
                string Platform = Response["Platform"].ToString();
                DateTime StartDate =Convert.ToDateTime( Response["StartDate"].ToString());
                DateTime CompletedOn = Convert.ToDateTime( Response["CompletedOn"].ToString());
                string Json = Response.ToString();
                List<Page> page1 = JsonConvert.DeserializeObject<List<Page>>(Response["page"].ToString());
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.SetBookReading(UserId, BookId, IsCompleted, CurrentPage, page1, Environment, Platform, StartDate, CompletedOn,Json);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        [HttpGet]
        [ActionName("getbookactivity")]
        public BookActivity GetBookActivity(int UserId, int BookId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetBookActivity(UserId, BookId);
            }
            catch
            {
                return (new BookActivity() { Status = BookStatus.Error });
            }

        }

        [HttpPost]
        [ActionName("setbookactivity")]
        public GenericStatus SetBookActivity(JObject Response)
        {
            try
            {
                if (Response["UserId"] == null || Response["BookId"] == null || Response["Json"] == null || Response["IsActivityDone"] == null || Response["Platform"] == null || Response["Environment"] == null || Response["CompletedOn"] == null || Response["CompletionTime"] == null)
                    return GenericStatus.Error;
                BookActivity activity = new BookActivity();
                activity.UserId = Convert.ToInt32(Response["UserId"]);
                activity.BookId = Convert.ToInt32(Response["BookId"]);
                activity.Json = Convert.ToString(Response["Json"]);
                activity.IsActivityDone = Convert.ToBoolean(Response["IsActivityDone"]);
                activity.Platform = Convert.ToString(Response["Platform"]);
                activity.Environment = Convert.ToString(Response["Environment"]);
                activity.CompletedOn = Convert.ToDateTime(Response["CompletedOn"]);
                activity.CompletionTime = Convert.ToInt32(Response["CompletionTime"]);
                string Json = Response.ToString();
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.SetBookActivity(activity, Json);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        [HttpGet]
        [ActionName("bookcompleted")]
        public BookRead BookCompleted(int UserId, int BookId)
        {
            _FISEService._currentUserId = _currentUserId;
            return _FISEService.BookCompleted(UserId, BookId);
        }

        [HttpPost]
        [ActionName("setbookreview")]
        public GenericStatus SetBookReview(JObject Response)
        {
            try
            {
                if (Response["UserId"] == null || Response["BookId"] == null || Response["Json"] == null || Response["IsReviewDone"] == null || Response["Platform"] == null || Response["Environment"] == null || Response["CompletedOn"] == null || Response["CompletionTime"] == null || Response["Rating"] == null)
                    return GenericStatus.Error;
                BookActivity activity = new BookActivity();
                activity.UserId = Convert.ToInt32(Response["UserId"]);
                activity.BookId = Convert.ToInt32(Response["BookId"]);
                activity.Json = Convert.ToString(Response["Json"]);
                activity.IsActivityDone = Convert.ToBoolean(Response["IsReviewDone"]);
                activity.Platform = Convert.ToString(Response["Platform"]);
                activity.Environment = Convert.ToString(Response["Environment"]);
                activity.CompletedOn = Convert.ToDateTime(Response["CompletedOn"]);
                activity.CompletionTime = Convert.ToInt32(Response["CompletionTime"]);
                activity.Rating = Convert.ToInt32(Response["Rating"]);
                string Json = Response.ToString();
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.SetBookReview(activity, Json);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        [HttpGet]
        [ActionName("getuac")]
        public UAC GetUAC(int UserId)
        {
            try
            {
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.GetUAC(UserId);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ActionName("updatebrowsingtime")]
        public bool UpdateBrowsingTime(JObject Response)
        {
            try
            {
                if (Response["UserId"] == null || Response["TimeSpent"] == null)
                    return false;

                int UserId = Convert.ToInt32(Response["UserId"]);
                int TimeSpent = Convert.ToInt32(Response["TimeSpent"]);
                string Plateform = Convert.ToString(Response["Plateform"]);
                string Environment = Convert.ToString(Response["Environment"]);
                DateTime EndDate = Convert.ToDateTime(Response["EndDate"]);
                string SessionId = Convert.ToString(Response["SessionId"]);
                string Callfrom = Convert.ToString(Response["Callfrom"]);
                _FISEService._currentUserId = _currentUserId;
                return _FISEService.UpdateBrowsingTime(UserId, TimeSpent, EndDate, Plateform, Environment, SessionId, Callfrom);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    public class MyCustomActionFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext filterContext)
        {
            try
            {
                List<string> myheader = filterContext.Request.Headers.GetValues("CurrentUserId").ToList();
                FISEAPIController._currentUserId = Convert.ToInt32(myheader.FirstOrDefault());
            }
            catch
            {
                FISEAPIController._currentUserId = 0;
            }
        }
    }
}