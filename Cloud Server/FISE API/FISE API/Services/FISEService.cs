using Newtonsoft.Json;
using FISE_API.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Configuration;
using FISE_API.Models;
using FISE_API.Services.EmailService;
using System.IO;
using System.Xml.Serialization;

namespace FISE_API.Services
{
    public class FISEService
    {
        private DataProvider _DataProvider;
        private string _siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
        private string _siteEmail = ConfigurationManager.AppSettings["SiteAdminEmail"];
        private string _userImportPassword = ConfigurationManager.AppSettings["UserImportPassword"];
        public int _currentUserId { get; set; }
        public FISEService()
        {
            _DataProvider = new DataProvider();
        }


        public UserResult AddNewUser(string Email, string MobileNo, string UserType)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.AddNewUser(Email, MobileNo, UserType);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "AddNewUser", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
        }

        public int? Register(User user)
        {
            if (user.RegistrationDate == null)
                user.RegistrationDate = DateTime.Now;
            try
            {
                string PasswordSalt = EncryptionService.CreateSaltKey(5);
                user.PasswordSalt = PasswordSalt;
                user.Password = EncryptionService.CreatePasswordHash(user.Password, PasswordSalt);
                _DataProvider._currentUserId = _currentUserId;
                int? id = _DataProvider.RegisterUser(user);
                return id;
            }
            catch
            {
                return default(int);
            }
        }

        public JObject GetRegisteration(string Token, int ParentId = 0)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                JObject _ConsumerResult = _DataProvider.GetRegisteration(Token, ParentId);
                return _ConsumerResult;
            }
            catch (Exception ex)
            {
                return JObject.FromObject(new UserBase { returnStatus = (int)UserStatus.Error });
            }
        }

        public ReturnStatus VerifyMobile(string Email, string MobileNo, string Type, int EntityId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return new ReturnStatus { returnStatus = _DataProvider.VerifyMobile(Email, MobileNo, Type, EntityId) };
            }
            catch (Exception ex)
            {
                return new ReturnStatus { returnStatus = 0 };
            }
        }
        public string ValidateLogin(string Username, string Password, string AppType)
        {
            JObject _User = null;
            _User = GetUserByUserName(Username);

            if (_User == null)
                return JsonConvert.SerializeObject(new { Status = UserStatus.UserAccountNotExist });
            string pwd = string.Empty;
            dynamic UserObj = _User;

            if (AppType == "cloud" && ((string)UserObj.Role).ToLower() == "student")
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.LoginIsNotAllowed });
            }
            if (AppType == "app" && ((string)UserObj.Role).ToLower() == "parent")
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.LoginIsNotAllowed });
            }
            else if (AppType == "app" && ((string)UserObj.Role).ToLower() == "student" && (int)UserObj.IsSubscriptionExpired == 1)
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.SubscriptionExpired });
            }
            if ((int)UserObj.IsLoginEnabled == 0)
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.AccountIsDisabled });
            }

            if (!(bool)UserObj.Status)
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.AccountIsNotActive });
            }
            pwd = EncryptionService.CreatePasswordHash(Password, (String)UserObj.PasswordSalt);
            bool isValid = pwd == (String)UserObj.Password;
            if (isValid)
            {
                _DataProvider._currentUserId = _currentUserId;
                _DataProvider.UpdateLastLogin((int)UserObj.UserId);
                return JsonConvert.SerializeObject(new { Status = UserStatus.Sucess, User = _User });
            }
            else
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.WrongCredentails });
            }
        }

        public string BrowserValidateLogin(string Username, string Password, string AppType, string Plateform, string Browser, string SessionId, string IPAddress)
        {
            JObject _User = null;
            _User = GetUserByUserName(Username);

            if (_User == null)
                return JsonConvert.SerializeObject(new { Status = UserStatus.UserAccountNotExist });
            string pwd = string.Empty;
            dynamic UserObj = _User;

            if (AppType == "cloud" && ((string)UserObj.Role).ToLower() == "student")
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.LoginIsNotAllowed });
            }
            if (AppType == "app" && ((string)UserObj.Role).ToLower() == "parent")
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.LoginIsNotAllowed });
            }
            else if (AppType == "app" && ((string)UserObj.Role).ToLower() == "student" && (int)UserObj.IsSubscriptionExpired == 1)
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.SubscriptionExpired });
            }
            if ((int)UserObj.IsLoginEnabled == 0)
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.AccountIsDisabled });
            }

            if (!(bool)UserObj.Status)
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.AccountIsNotActive });
            }
            pwd = EncryptionService.CreatePasswordHash(Password, (String)UserObj.PasswordSalt);
            bool isValid = pwd == (String)UserObj.Password;
            if (isValid)
            {
                _DataProvider._currentUserId = _currentUserId;
                string _SessionId = SessionId;
                if (string.IsNullOrEmpty(SessionId))
                    _SessionId=Guid.NewGuid().ToString();
                _DataProvider.CheckBrowserLogin((int)UserObj.UserId, _SessionId, Plateform, Browser, IPAddress);
                _User.Add("SessionId", _SessionId);
                return JsonConvert.SerializeObject(new { Status = UserStatus.Sucess, User = _User });
            }
            else
            {
                return JsonConvert.SerializeObject(new { Status = UserStatus.WrongCredentails });
            }
        }

        public UserStatus ValidateSchoolAdmin(string Username, string Password)
        {
            JObject _User = null;
            _User = GetUserByUserName(Username);

            if (_User == null)
                return UserStatus.UserAccountNotExist;
            string pwd = string.Empty;
            dynamic UserObj = _User;

            if ((int)UserObj.IsLoginEnabled == 0)
            {
                return UserStatus.AccountIsDisabled;
            }

            if (!(bool)UserObj.Status)
            {
                return UserStatus.AccountIsNotActive;
            }
            pwd = EncryptionService.CreatePasswordHash(Password, (String)UserObj.PasswordSalt);
            bool isValid = pwd == (String)UserObj.Password;
            if (isValid)
            {
                if ((String)UserObj.Role != "schooladmin")
                    return UserStatus.Error;
                else
                    return UserStatus.Sucess;
            }
            else
            {
                return UserStatus.WrongCredentails;
            }
        }
        public JObject GetUserByUserName(string Username)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                DataSet ds = _DataProvider.GetUserByUserName(Username);

                if (ds != null)
                {
                    var userList =
                        from c in ds.Tables[0].AsEnumerable()
                        select new
                        {
                            UserId = c.Field<int>("UserId"),
                            Username = c.Field<string>("UserName"),
                            Email = c.Field<string>("Email"),
                            Status = c.Field<bool>("Status"),
                            Password = c.Field<string>("Password"),
                            PasswordSalt = c.Field<string>("PasswordSalt"),
                            RegistrationDate = c.Field<DateTime?>("RegistrationDate"),
                            Role = c.Field<string>("SystemName"),
                            SchoolName = c.IsNull("SchoolName") ? "" : c.Field<string>("SchoolName"),
                            Grade = c.IsNull("Grade") ? "" : c.Field<string>("Grade"),
                            SubSection = c.IsNull("SubSection") ? "" : c.Field<string>("SubSection"),
                            GradeId = c.IsNull("GradeId") ? 0 : c.Field<int>("GradeId"),
                            SubSectionId = c.IsNull("SubSectionId") ? 0 : c.Field<int>("SubSectionId"),
                            FirstName = c.IsNull("FirstName") ? "" : c.Field<string>("FirstName"),
                            LastName = c.IsNull("LastName") ? "" : c.Field<string>("LastName"),
                            AvatarImage = c.IsNull("AvatarImage") ? "" : c.Field<string>("AvatarImage"),
                            AvatarId = c.IsNull("AvatarId") ? 0 : c.Field<int>("AvatarId"),
                            MobileNumber = c.Field<string>("MobileNo"),
                            Gender = c.Field<string>("Gender"),
                            DateOfBirth = c.Field<DateTime?>("DateOfBirth"),
                            AddressLine1 = c.Field<string>("AddressLine1"),
                            AddressLine2 = c.Field<string>("AddressLine2"),
                            State = c.Field<string>("State"),
                            Country = c.Field<string>("Country"),
                            PinCode = c.Field<string>("PinCode"),
                            City = c.Field<string>("City"),
                            IsLoginEnabled = c.Field<int>("IsLoginEnabled"),
                            IsSubscriptionExpired = c.Field<int>("IsSubscriptionExpired"),
                            SchoolUId = c.Field<string>("SchoolUId")
                        };
                    JObject _user = JObject.FromObject(userList.ToList().FirstOrDefault());
                    return _user;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public JObject GetUserById(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                DataSet ds = _DataProvider.GetUserById(UserId);

                if (ds != null)
                {
                    var userList =
                        from c in ds.Tables[0].AsEnumerable()
                        select new
                        {
                            UserId = c.Field<int>("UserId"),
                            Username = c.Field<string>("UserName"),
                            Email = c.Field<string>("Email"),
                            Status = c.Field<bool>("Status"),
                            Password = c.Field<string>("Password"),
                            PasswordSalt = c.Field<string>("PasswordSalt"),
                            RegistrationDate = c.Field<DateTime?>("RegistrationDate"),
                            Role = c.Field<string>("SystemName"),
                            SchoolName = c.IsNull("SchoolName") ? "" : c.Field<string>("SchoolName"),
                            Grade = c.IsNull("Grade") ? "" : c.Field<string>("Grade"),
                            SubSection = c.IsNull("SubSection") ? "" : c.Field<string>("SubSection"),
                            GradeId = c.IsNull("GradeId") ? 0 : c.Field<int>("GradeId"),
                            SubSectionId = c.IsNull("SubSectionId") ? 0 : c.Field<int>("SubSectionId"),
                            FirstName = c.IsNull("FirstName") ? "" : c.Field<string>("FirstName"),
                            LastName = c.IsNull("LastName") ? "" : c.Field<string>("LastName"),
                            AvatarImage = c.IsNull("AvatarImage") ? "" : c.Field<string>("AvatarImage"),
                            AvatarId = c.IsNull("AvatarId") ? 0 : c.Field<int>("AvatarId"),
                            MobileNumber = c.Field<string>("MobileNo"),
                            Gender = c.Field<string>("Gender"),
                            DateOfBirth = c.Field<DateTime?>("DateOfBirth"),
                            AddressLine1 = c.Field<string>("AddressLine1"),
                            AddressLine2 = c.Field<string>("AddressLine2"),
                            State = c.Field<string>("State"),
                            Country = c.Field<string>("Country"),
                            PinCode = c.IsNull("PinCode") ? "0" : c.Field<string>("PinCode"),
                            City = c.Field<string>("City"),
                            IsLoginEnabled = c.Field<int>("IsLoginEnabled"),
                            CreationDate = c.Field<DateTime?>("CreationDate")
                        };
                    JObject _user = JObject.FromObject(userList.ToList().FirstOrDefault());
                    return _user;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public UserResult SendPasswordRecovery(string Username, string Url)
        {
            JObject _User = null;
            _DataProvider._currentUserId = _currentUserId;
            _User = GetUserByUserName(Username);

            if (_User == null)
                return new UserResult { Status = UserStatus.UserAccountNotExist };
            try
            {
                dynamic UserObj = _User;
                return _DataProvider.SendPasswordRecovery((int)UserObj.UserId, Url);
            }
            catch
            {
                return new UserResult { Status = UserStatus.Error };
            }
        }

        public GenericStatus ValidateUserToken(string Token, string Type)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.ValidateUserToken(Token, Type);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        public UserStatus PasswordRecoverySubmit(string Token, string NewPassword)
        {
            try
            {
                string PasswordSalt = EncryptionService.CreateSaltKey(5);
                string _Password = EncryptionService.CreatePasswordHash(NewPassword, PasswordSalt);
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.PasswordRecoverySubmit(Token, _Password, PasswordSalt);
            }
            catch
            {
                return UserStatus.Error;
            }
        }

        public UserStatus ChangePassword(int UserId, string NewPassword, string OldPassword)
        {
            JObject _User = null;
            _User = GetUserById(UserId);

            if (_User == null)
                return UserStatus.UserAccountNotExist;
            string pwd = string.Empty;
            dynamic userObj = _User;
            pwd = EncryptionService.CreatePasswordHash(OldPassword, (String)userObj.PasswordSalt);
            bool isValid = pwd == (String)userObj.Password;
            if (isValid)
            {
                string PasswordSalt = EncryptionService.CreateSaltKey(5);
                string _Password = EncryptionService.CreatePasswordHash(NewPassword, PasswordSalt);
                _DataProvider._currentUserId = _currentUserId;
                int id = _DataProvider.ChangePassword(UserId, _Password, PasswordSalt);
                if (id == 1)
                {
                    return UserStatus.Sucess;
                }
                else
                {
                    return UserStatus.Error;
                }
            }
            else
            {
                return UserStatus.WrongCredentails;
            }
        }

        public UserStatus UpdateUserProfile(UserProfile _User)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UpdateUserProfile(_User);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UpdateUserProfile", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return UserStatus.Error;
            }
        }

        public SchoolStatus CreateSchool(School _School)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.CreateSchool(_School);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "CreateSchool", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolStatus.Error;
            }
        }

        public SchoolResult GetSchoolProfileDetails(string SchoolUId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                SchoolResult _SchoolResult = _DataProvider.GetSchoolProfileDetails(SchoolUId);
                return _SchoolResult;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetSchoolProfileDetails", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new SchoolResult { Status = SchoolStatus.Error };
            }
        }

        public SchoolData GetSchoolByUId(string SchoolUId, int PageIndex, int PageSize)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                SchoolData _SchoolData = _DataProvider.GetSchoolByUId(SchoolUId, PageIndex, PageSize);
                return _SchoolData;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetSchoolByUId", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public SchoolStatus UpdateSchool(School _School)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UpdateSchool(_School);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UpdateSchool", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolStatus.Error;
            }
        }

        public PagedList<UserCommon> GetSchoolAdmins(string SchoolUId, int PageIndex, int PageSize, string SearchTxt)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<UserCommon> _SchoolAdmins = _DataProvider.GetSchoolAdmins(SchoolUId, PageIndex, PageSize, SearchTxt);
                return _SchoolAdmins;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetSchoolAdmins", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public StudentModel GetSchoolAdmin(string SchoolUId, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                StudentModel _SchoolAdmin = _DataProvider.GetSchoolAdmin(SchoolUId, UserId);
                return _SchoolAdmin;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetSchoolAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public UserResult AddSchoolAdmins(string SchoolUId, string Email, string MobileNo)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.AddSchoolAdmins(SchoolUId, Email, MobileNo);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "AddSchoolAdmins", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
        }

        public PagedList<School> GetSchoolList(int PageIndex, int PageSize, string SearchTxt)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<School> _Schools = _DataProvider.GetSchoolList(PageIndex, PageSize, SearchTxt);
                return _Schools;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetSchoolList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public string CheckDevice(string DeviceDetails)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                string _DeviceEnvironment = _DataProvider.CheckDevice(DeviceDetails);
                return _DeviceEnvironment;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "CheckDevice", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public List<Avatar> GetAvatar(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetAvatar(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetAvatar", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public List<UserEvent> GetEvents(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetEvents(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetEvents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;

            }
        }

        public GenericStatus UpdateViewEvents(string EventIds, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UpdateViewEvents(EventIds, UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UpdateViewEvents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return GenericStatus.Error;
            }
        }

        public AddDeviceModel AddDevice(DeviceDetail _Device)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.AddDevice(_Device);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "AddDevice", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new AddDeviceModel() { Status = DeviceStatus.Error };
            }
        }
        public bool AddEditAvatar(AddEditAvatar _Avatar)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.AddEditAvatar(_Avatar);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "AddEditAvatar", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
        }
        public bool SaveRating(BookRating _Rating)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SaveRating(_Rating);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "SaveRating", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
        }

        public GenericStatus RemoveUsersFromSchool(string UserIds, int SchoolId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.RemoveUsersFromSchool(UserIds, SchoolId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "RemoveUsersFromSchool", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return GenericStatus.Error;
            }
        }

        public PagedList<StudentModel> GetStudentsOfSchool(string SchoolUId, int PageSize, int PageIndex, string SearchTxt, string Grade)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<StudentModel> _Students = _DataProvider.GetStudentsOfSchool(SchoolUId, PageSize, PageIndex, SearchTxt, Grade);
                return _Students;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetStudentsOfSchool", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public StudentResult GetStudentOfSchool(int SchoolId, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                StudentResult _Result = _DataProvider.GetStudentOfSchool(SchoolId, UserId);
                return _Result;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetStudentOfSchool", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new StudentResult { Status = UserStatus.Error };
            }
        }

        public PagedList<User> GetELibraryAdminsList(int PageIndex, int PageSize, string SearchTxt)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<User> _ElibAdmins = _DataProvider.GetELibraryAdminsList(PageIndex, PageSize, SearchTxt);
                return _ElibAdmins;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetELibraryAdminsList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public StudentParentResult GetStudentById(string SchoolUId, int UserId)
        {
            StudentParentResult _StudentResult = new StudentParentResult();
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                _StudentResult = _DataProvider.GetStudentById(SchoolUId, UserId);
                return _StudentResult;
            }
            catch (Exception ex)
            {
                _StudentResult.APIStatus = UserStatus.Error;
            }
            return _StudentResult;
        }

        public BooksListResult GetBooksListWithFilter(int PageIndex, int PageSize)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                BooksListResult _BooksList = _DataProvider.GetBooksListWithFilter(PageIndex, PageSize);
                return _BooksList;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetBooksListWithFilter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public PagedList<Book> GetBooksListByFilter(int PageIndex, int PageSize, string SearchTxt, string SubSection, string Language, string BookType, bool HasAnimation, bool HasReadAloud, bool HasActivity)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<Book> _BooksList = _DataProvider.GetBooksListByFilter(PageIndex, PageSize, SearchTxt, SubSection, Language, BookType, HasAnimation, HasReadAloud, HasActivity);
                return _BooksList;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetBooksListByFilter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<ExportBook> GetBooksListForExport(string SubSection, string Language, string BookType, bool HasAnimation, bool HasReadAloud, bool HasActivity)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                List<ExportBook> _BooksList = _DataProvider.GetBooksListForExport(SubSection, Language, BookType, HasAnimation, HasReadAloud, HasActivity);
                return _BooksList;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetBooksListForExport", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public StudentImportExportInput ImportStudents(List<StudentImportExport> Students, string SchoolUId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                StudentImportExportInput _InsertStatus = _DataProvider.ImportStudents(Students, SchoolUId);
                return _InsertStatus;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "ImportStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public ChildImportExportInput ImportChildren(List<ChildImport> Students, string SchoolUId)
        {
            try
            {
                string PasswordSalt = EncryptionService.CreateSaltKey(5);
                string Password = _userImportPassword;
                Password = EncryptionService.CreatePasswordHash(Password, PasswordSalt);
                _DataProvider._currentUserId = _currentUserId;
                ChildImportExportInput _InsertStatus = _DataProvider.ImportChildren(Students, SchoolUId, Password, PasswordSalt);
                return _InsertStatus;
            }
            catch (Exception ex)
            {
                //_DataProvider.InsertLog(_currentUserId, "ImportStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public List<StudentImportExport> ValidateImportStudents(List<StudentImportExport> Students)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                List<StudentImportExport> _InsertStatus = _DataProvider.ValidateImportStudents(Students);
                return _InsertStatus;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "ValidateImportStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public List<ChildImport> ValidateImportChildren(List<ChildImport> Students)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                List<ChildImport> _InsertStatus = _DataProvider.ValidateImportChildren(Students);
                return _InsertStatus;
            }
            catch (Exception ex)
            {
                //_DataProvider.InsertLog(_currentUserId, "ValidateImportStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public StudentImportExportInput UpdateMultipleStudents(List<StudentImportExport> Students, string SchoolUId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                StudentImportExportInput _UpdateStatus = _DataProvider.UpdateMultipleStudents(Students, SchoolUId);
                return _UpdateStatus;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UpdateMultipleStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public UserStatus RegisterStudent(StudentRegistrationModel student)
        {
            try
            {
                string PasswordSalt = EncryptionService.CreateSaltKey(5);
                student.PasswordSalt = PasswordSalt;
                student.Password = EncryptionService.CreatePasswordHash(student.Password, PasswordSalt);
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.RegisterStudent(student);
                // return id;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "RegisterStudent", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return UserStatus.Error;
            }
        }

        public bool IsUsernameUnique(string userName)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.IsUsernameUnique(userName);
            }
            catch (Exception ex)
            {
                return default(bool);
            }
        }

        public SchoolAdminDisableStatus DisableSchoolAdmin(int SchoolAdminId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.DisableSchoolAdmin(SchoolAdminId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "DisableSchoolAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolAdminDisableStatus.Error;
            }
        }
        public ParentStudentDisableStatus DisableParentStudent(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.DisableParentStudent(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "DisableParentStudent", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return ParentStudentDisableStatus.Error;
            }
        }

        public ElibAdminDisableStatus DisableELibAdmin(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.DisableELibAdmin(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "DisableELibAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return ElibAdminDisableStatus.Error;
            }
        }

        public BookDisableStatus DisableBook(int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.DisableBook(BookId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "DisableBook", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return BookDisableStatus.Error;
            }
        }

        public SchoolDisableStatus DisableSchool(int SchoolId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.DisableSchool(SchoolId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "DisableSchool", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolDisableStatus.Error;
            }
        }

        public BooksDetailsResult GetBookDetailsById(int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetBookDetailsById(BookId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetBookDetailsById", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public GenericStatus UpdateBooKMetaData(Book _book)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UpdateBooKMetaData(_book);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UpdateBooKMetaData", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return GenericStatus.Error;
            }
        }

        public string GetBooksCatlog()
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                UserBooksDetail books = _DataProvider.GetBooksCatlog();
                if (books != null)
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.NewLineHandling = NewLineHandling.None;
                    settings.Indent = false;
                    StringWriter _StringWriter = new StringWriter();
                    XmlWriter writer = XmlWriter.Create(_StringWriter, settings);

                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    XmlSerializer MySerializer = new XmlSerializer(books.GetType());
                    MySerializer.Serialize(writer, books, namespaces);
                    return _StringWriter.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetBooksCatlog", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public bool ReleaseUserBooks(int UserId, string BookIds, int DeviceId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.ReleaseUserBooks(UserId, BookIds, DeviceId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "ReleaseUserBooks", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
        }
        public bool ReleaseUserDevices(int UserId, string DeviceIds)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.ReleaseUserDevices(UserId, DeviceIds);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "ReleaseUserDevices", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
        }
        public GenericStatus UserDownloadBook(int UserId, int BookId, int DeviceId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UserDownloadBook(UserId, BookId, DeviceId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UserDownloadBook", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return GenericStatus.Error;
            }
        }
        public ReadBookStatus ReadBook(int UserId, string DeviceDetails, int MaxCopies, int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.ReadBook(UserId, DeviceDetails, MaxCopies, BookId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "ReadBook", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return ReadBookStatus.Error;
            }
        }

        public GenericStatus ReadLater(int userId, int bookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.ReadLater(userId, bookId);
            }
            catch (Exception ex)
            {
                return GenericStatus.Error;
            }
        }

        public SearchResult Search(string Role, int PageIndex, int PageSize, string SearchTxt, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.Search(Role, PageIndex, PageSize, SearchTxt, UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "Search", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public PagedList<StudentModel> SearchStudent(int PageIndex, int PageSize, string SearchTxt, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SearchStudent(PageIndex, PageSize, SearchTxt, UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "SearchStudent", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public PagedList<StudentModel> SearchSchoolAdmin(int PageIndex, int PageSize, string SearchTxt)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SearchSchoolAdmin(PageIndex, PageSize, SearchTxt);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "SearchSchoolAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public PagedList<School> SearchSchoolList(int PageIndex, int PageSize, string SearchTxt)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<School> _Schools = _DataProvider.SearchSchoolList(PageIndex, PageSize, SearchTxt);
                return _Schools;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "SearchSchoolList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public PagedList<Book> SearchBooksList(int PageIndex, int PageSize, string SearchTxt)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<Book> _BooksList = _DataProvider.SearchBooksList(PageIndex, PageSize, SearchTxt);
                return _BooksList;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "SearchBooksList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public string GetBookDownloadUrl(int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                string Blobpath = _DataProvider.GetBlobPathOfBook(BookId);
                if (!String.IsNullOrEmpty(Blobpath))
                {
                    return new BlobService().GetSASUri(Blobpath);
                }
            }
            catch
            {
            }

            return null;
        }

        public ParentDashboardResult GetParentDashbord(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetParentDashbord(UserId);

            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetParentDashbord", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public ElibSuperAdminDashboardResult GetElibSuperAdminDashboard()
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetElibSuperAdminDashboard();
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetElibSuperAdminDashboard", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public UsernameRecoveryStatus UserNameRecovery(string Email)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UserNameRecovery(Email);
            }
            catch
            {
                return UsernameRecoveryStatus.Error;
            }
        }

        public SchoolRegistrationEmailStatus SchoolRegistrationEmail(int SchoolId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SchoolRegistrationEmail(SchoolId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "SchoolRegistrationEmail", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolRegistrationEmailStatus.Error;
            }
        }

        public StudentProfileResult GetStudentProfile(int UserId, string SchoolUId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetStudentProfile(UserId, SchoolUId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetStudentProfile", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public JObject GetParentProfile(int UserId, string SchoolUId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                DataSet ds = _DataProvider.GetParentProfile(UserId, SchoolUId);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var userList =
                        from c in ds.Tables[0].AsEnumerable()
                        select new
                        {
                            UserId = c.Field<int>("UserId"),
                            Username = c.Field<string>("UserName"),
                            Email = c.Field<string>("Email"),
                            Status = c.Field<bool>("Status"),
                            Password = c.Field<string>("Password"),
                            PasswordSalt = c.Field<string>("PasswordSalt"),
                            RegistrationDate = c.Field<DateTime?>("RegistrationDate"),
                            Role = c.Field<string>("SystemName"),
                            SchoolName = c.IsNull("SchoolName") ? "" : c.Field<string>("SchoolName"),
                            FirstName = c.IsNull("FirstName") ? "" : c.Field<string>("FirstName"),
                            LastName = c.IsNull("LastName") ? "" : c.Field<string>("LastName"),
                            MobileNumber = c.Field<string>("MobileNo"),
                            Gender = c.Field<string>("Gender"),
                            DateOfBirth = c.Field<DateTime?>("DateOfBirth"),
                            CreationDate = c.Field<DateTime?>("CreationDate"),
                            LastLoginDate = c.Field<DateTime?>("LastLoginDate")
                        };
                    JObject _user = JObject.FromObject(userList.ToList().FirstOrDefault());
                    return _user;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetParentProfile", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public UserStatus UpdateStudent(StudentProfileResult _Student)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UpdateStudent(_Student);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UpdateStudent", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return UserStatus.Error;
            }
        }

        public UserStatus UserRegistrationEmail(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UserRegistrationEmail(UserId);
            }
            catch
            {
                return UserStatus.Error;
            }
        }

        public UserResult AddElibAdmin(string Email, string MobileNo)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.AddElibAdmin(Email, MobileNo);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "AddElibAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
        }

        public User GetELibraryAdmin(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                User _elibAdmin = _DataProvider.GetELibraryAdmin(UserId);
                return _elibAdmin;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetELibraryAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public SchoolData GetSchoolAdminDashboard(int UserId, int PageIndex, int PageSize, string SchoolUId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                SchoolData _SchoolData = _DataProvider.GetSchoolAdminDashboard(UserId, PageIndex, PageSize, SchoolUId);
                return _SchoolData;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetSchoolAdminDashboard", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public GenericStatus CreateHelpItem(string UserMessage, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                HelpItem userdata = _DataProvider.CreateHelpItem(UserMessage, UserId);
                if (userdata != null)
                {
                    if (String.IsNullOrEmpty(userdata.UserEmail) || String.IsNullOrEmpty(userdata.ReferenceId))
                    {
                        return GenericStatus.Error;
                    }
                    else
                    {
                        if (EmailSender.SendHelpAcknowledgementEmail(userdata.ReferenceId, userdata.UserEmail, userdata.Query, userdata.CreatedOn.ToString("dd/MM/yyyy"), userdata.Role, userdata.StudentFirstName, userdata.StudentLastName, _siteUrl, string.Empty) && EmailSender.SendHelpAcknowledgementEmail(userdata.ReferenceId, userdata.UserEmail, userdata.Query, userdata.CreatedOn.ToString("dd/MM/yyyy"), String.Empty, String.Empty, String.Empty, _siteUrl, _siteEmail))
                        {
                            return GenericStatus.Sucess;
                        }
                    }
                }
                return GenericStatus.Error;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "CreateHelpItem", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return GenericStatus.Error;
            }
        }

        public PagedList<StudentModel> GetStudentsOfSchoolAdmin(string SchoolUId, int PageSize, int PageIndex, string Grade)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<StudentModel> _Students = _DataProvider.GetStudentsOfSchoolAdmin(SchoolUId, PageSize, PageIndex, Grade);
                return _Students;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetStudentsOfSchoolAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public RegistrationAndLogin GetRegistrationAndLoginReport(string UserType, string SchoolIds, int PageSize, int PageIndex)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                RegistrationAndLogin _Students = _DataProvider.GetRegistrationAndLoginReport(UserType, SchoolIds, PageSize, PageIndex);
                return _Students;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetRegistrationAndLoginReport", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public PagedList<StudentModel> GetRegistrationAndLoginReportList(string UserType, string SchoolIds, int PageSize, int PageIndex, string Type)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                PagedList<StudentModel> _Students = _DataProvider.GetRegistrationAndLoginReportList(UserType, SchoolIds, PageSize, PageIndex, Type);
                return _Students;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetRegistrationAndLoginReportList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public PagedList<StudentModel> GetReport1SchoolAdminList(string SchoolUId, int PageSize, int PageIndex, string Type, int GradeId, string Section)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport1SchoolAdminList(SchoolUId, PageSize, PageIndex, Type, GradeId, Section);

            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport1SchoolAdminList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report1SchoolAdmin GetReport1SchoolAdmin(string SchoolUId, int PageSize, int PageIndex, bool IsExport = false)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport1SchoolAdmin(SchoolUId, PageSize, PageIndex, IsExport);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport1SchoolAdmin", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<School> GetSchools(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                List<School> _Schools = _DataProvider.GetSchools(UserId);
                return _Schools;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetSchools", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<StudentImportExport> ValidateBulkUpdateStudents(List<StudentImportExport> Students, string SchoolUId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                List<StudentImportExport> _InsertStatus = _DataProvider.ValidateBulkUpdateStudents(Students, SchoolUId);
                return _InsertStatus;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "ValidateBulkUpdateStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        public List<StudentImportExport> GetStudentsOfSchoolForExport(string SchoolUId, int PageSize, int PageIndex, string SearchTxt, string Grade)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                List<StudentImportExport> _Students = _DataProvider.GetStudentsOfSchoolForExport(SchoolUId, PageSize, PageIndex, SearchTxt, Grade);
                return _Students;
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetStudentsOfSchoolForExport", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        #region "Reports"

        public Report6 GetReport6Filter()
        {
            Report6 _filters = new Report6();
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                _filters = _DataProvider.GetReport6Filter();
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport6Filter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _filters.Status = Report6Status.Error;
            }
            return _filters;
        }

        public Report6 GetReport6(string SubsectionIds, string LanguageIds)
        {
            Report6 _filters = new Report6();
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                _filters = _DataProvider.GetReport6(SubsectionIds, LanguageIds);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport6", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _filters.Status = Report6Status.Error;
            }
            return _filters;
        }

        public List<Book> GetReport6ForExport(string SubsectionIds, string LanguageIds)
        {
            List<Book> _filters = new List<Book>();
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                _filters = _DataProvider.GetReport6ForExport(SubsectionIds, LanguageIds);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport6ForExport", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _filters;
        }

        public Report7FilterModel GetReport7Fillters()
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport7Fillters();
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport7Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report7Model GetReport7(DateTime StartDate, DateTime EndDate, string SchoolIds, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport7(StartDate, EndDate, SchoolIds, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport7", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<Report7ExportModel> GetReport7Export(DateTime StartDate, DateTime EndDate, string SchoolIds, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport7Export(StartDate, EndDate, SchoolIds, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport7Export", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report2FilterModel GetReport2Fillters(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport2Fillters(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport2Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report3FilterModel GetReport3Fillters(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport3Fillters(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport3Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report4FilterModel GetReport4Fillters()
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport4Fillters();
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport4Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report3 GetReport3(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string BookIds, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport3(StartDate, EndDate, LanguageIds, SubsectionIds, BookIds, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport3", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report4 GetReport4(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string Publisher, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport4(StartDate, EndDate, LanguageIds, SubsectionIds, Publisher, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport4", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<Report8> GetReport8(DateTime StartDate, DateTime EndDate, string StudentIds, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport8(StartDate, EndDate, StudentIds, UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport8", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<Report8Export> GetReport8Export(DateTime StartDate, DateTime EndDate, string StudentIds, int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport8Export(StartDate, EndDate, StudentIds, UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport8Export", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<StudentModel> GetReport8Fillters(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport8Fillters(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport8Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report2 GetReport2(DateTime StartDate, DateTime EndDate, string SchoolIds, string GradeIds, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport2(StartDate, EndDate, SchoolIds, GradeIds, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport2", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report2StudentWise GetReport2StudentWise(DateTime StartDate, DateTime EndDate, string Section, int UserId, int GradeId, int SchoolId,string CallFrom)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport2StudentWise(StartDate, EndDate, Section, UserId, GradeId, SchoolId, CallFrom);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport2StudentWise", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<Report2Export> GetReport2Export(DateTime StartDate, DateTime EndDate, string SchoolIds, string GradeIds, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport2Export(StartDate, EndDate, SchoolIds, GradeIds, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport2Export", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report5FilterModel GetReport5Fillters(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport5Fillters(UserId);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport5Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public Report5 GetReport5(DateTime StartDate, DateTime EndDate, int SchoolId, string GradeIds)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport5(StartDate, EndDate, SchoolId, GradeIds);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport5", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<Report5Export> GetReport5Export(DateTime StartDate, DateTime EndDate, int SchoolId, string GradeIds)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport5Export(StartDate, EndDate, SchoolId, GradeIds);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport5Export", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<Report3Export> GetReport3Export(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string BookIds, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport3Export(StartDate, EndDate, LanguageIds, SubsectionIds, BookIds, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport3Export", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public List<Report3Export> GetReport4Export(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string Publisher, string City)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetReport4Export(StartDate, EndDate, LanguageIds, SubsectionIds, Publisher, City);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetReport4Export", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public string GetUsernameByToken(string token)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetUsernameByToken(token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SyncData(UserProgressForSync UserSyncData, string Type, int DataId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SyncUserData(UserSyncData, Type, DataId);

            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "SyncData", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
        }
        public int InsertSyncData(int UserId, string UserSyncData, string Type)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.InsertSyncUserData(UserId, UserSyncData, Type);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "InsertSyncData", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return 0;
            }
        }
        public string GetUserBooksDetails(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                UserDetails _UserDetails = _DataProvider.GetUserBooksDetails(UserId);
                if (_UserDetails != null)
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    //settings.NewLineHandling = NewLineHandling.None;
                    //settings.Indent = false;
                    StringWriter _StringWriter = new StringWriter();
                    XmlWriter writer = XmlWriter.Create(_StringWriter, settings);

                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    XmlSerializer MySerializer = new XmlSerializer(_UserDetails.GetType());
                    MySerializer.Serialize(writer, _UserDetails, namespaces);
                    return _StringWriter.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "GetUserBooksDetails", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }
        #endregion

        public UserResult UpdateCreatedUser(string Email, string MobileNo, int UserId, string UserType)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UpdateCreatedUser(Email, MobileNo, UserId, UserType);
            }
            catch (Exception ex)
            {
                _DataProvider.InsertLog(_currentUserId, "UpdateCreatedUser", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
        }

        public string GetLastemailSendDate(int UserId, string Type)
        {
            try
            {
                return _DataProvider.GetLastemailSendDate(UserId, Type);
            }
            catch
            {
                return "";
            }
        }

        public UserPasswordRecovery SendPasswordRecoveryReader(string Username, string Environment)
        {
            UserPasswordRecovery _Result = new UserPasswordRecovery();
            JObject _User = null;
            _DataProvider._currentUserId = _currentUserId;
            _User = GetUserByUserName(Username);

            if (_User == null)
            {
                _Result.Status = UserStatus.UserAccountNotExist;
                return _Result;
            }
            try
            {
                dynamic UserObj = _User;
                if (UserObj.Role.ToString().ToLower() == "student" && Environment.ToLower() == "school")
                {
                    _Result = _DataProvider.SendPasswordRecoveryReader((int)UserObj.UserId);
                    _Result.Status = UserStatus.MoreInfo;
                }
                else
                    _Result.Status = _DataProvider.SendPasswordRecovery((int)UserObj.UserId, "").Status;
            }
            catch
            {
                _Result.Status = UserStatus.Error;
            }
            return _Result;
        }

        public UserPasswordRecovery SendSchoolStudentPasswordRecovery(string UserName, int ToId)
        {
            UserPasswordRecovery _Result = new UserPasswordRecovery();

            try
            {
                _Result = _DataProvider.SendSchoolStudentPasswordRecovery(UserName, ToId);
            }
            catch
            {
                _Result.Status = UserStatus.Error;
            }
            return _Result;
        }

        public BookRead GetBookDetail(int UserId, int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetBookDetail(UserId, BookId);
            }
            catch
            {
                return (new BookRead() { Status = BookStatus.Error });
            }
        }

        public BookDisplay BookDisplay(int UserId, int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.BookDisplay(UserId, BookId);
            }
            catch
            {
                return (new BookDisplay() { Status = GenericStatus.Error });
            }
        }

        public GenericStatus SetBookReading(int UserId, int BookId, bool IsCompleted, int CurrentPage, List<Page> page, string Environment, string Platform, DateTime StartDate, DateTime CompletedOn, string Json)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SetBookReading(UserId, BookId, IsCompleted, CurrentPage, page, Environment, Platform, StartDate, CompletedOn, Json);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        public GenericStatus SetBookActivity(BookActivity activity, string Json)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SetBookActivity(activity, Json);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        public BookActivity GetBookActivity(int UserId, int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetBookActivity(UserId, BookId);
            }
            catch
            {
                return (new BookActivity() { Status = BookStatus.Error });
            }
        }

        public BookRead BookCompleted(int UserId, int BookId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.BookCompleted(UserId, BookId);
            }
            catch
            {
                return (new BookRead() { Status = BookStatus.Error });
            }
        }

        public GenericStatus SetBookReview(BookActivity review, string Json)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.SetBookReview(review, Json);
            }
            catch
            {
                return GenericStatus.Error;
            }
        }

        public UAC GetUAC(int UserId)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.GetUAC(UserId);
            }
            catch
            {
                return null;
            }
        }

        public bool UpdateBrowsingTime(int UserId, int TimeSpent,DateTime EndDate,string Plateform,string Environment,string SessionId,string Callfrom)
        {
            try
            {
                _DataProvider._currentUserId = _currentUserId;
                return _DataProvider.UpdateBrowsingTime(UserId, TimeSpent, EndDate, Plateform, Environment, SessionId, Callfrom);
            }
            catch
            {
                return false;
            }
        }

    }
}