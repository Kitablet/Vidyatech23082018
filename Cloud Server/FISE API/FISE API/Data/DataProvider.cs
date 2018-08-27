using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using FISE_API.Models;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Web;
namespace FISE_API.Data
{
    public class DataProvider
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["FISE_APIConString"].ConnectionString;
        public int _currentUserId { get; set; }


        public UserResult AddNewUser(string Email, string MobileNo, string UserType)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spAddNewUser", con))
                    {
                        string Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                        command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = MobileNo;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = Token;
                        command.Parameters.Add("@UserType", SqlDbType.NVarChar).Value = UserType;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;

                        if (_Status == 1)
                        {
                            InsertLog(_currentUserId, "AddNewUser", "New user added", String.Format("New user added with Type={0},Email={1} and Mobile={2}", UserType, Email, MobileNo), UserStatus.Sucess.ToString());
                            User _User = new User();
                            _User.Token = Token;
                            return new UserResult { User = _User, Status = UserStatus.Sucess };
                        }
                        else if (_Status == 0)
                        {
                            InsertLog(_currentUserId, "AddNewUser", "Failed to add new user", "Failed to add new user", UserStatus.Error.ToString());
                            return new UserResult { Status = UserStatus.Error };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "AddNewUser", String.Format("Failed to add new user with Type={0},Email={1} and Mobile={2}", UserType, Email, MobileNo), ex.Message, UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
            return new UserResult { Status = UserStatus.Error };
        }

        public int? RegisterUser(User _User)
        {
            int? _id = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spRegisterUser", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_User.FirstName);
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_User.LastName);
                        command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = _User.Username.ToLower();
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = _User.Password;
                        command.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar).Value = _User.PasswordSalt;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = _User.Token;
                        command.Parameters.Add("@AddressLine1", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_User.AddressLine1);
                        command.Parameters.Add("@AddressLine2", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_User.AddressLine2);
                        command.Parameters.Add("@State", SqlDbType.NVarChar).Value = _User.State;
                        command.Parameters.Add("@Country", SqlDbType.NVarChar).Value = _User.Country;
                        command.Parameters.Add("@PinCode", SqlDbType.NVarChar).Value = _User.PinCode;
                        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = _User.PhoneNumber;
                        command.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = _User.Gender;
                        command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = _User.DateOfBirth;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = _User.City;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        if (_id != null && _id > 0)
                        {
                            if(_id==1)
                            InsertLog(_currentUserId, "RegisterUser", "New user registered", String.Format("New user registered with username={0}", _User.Username.ToLower()), UserStatus.Sucess.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "RegisterUser", String.Format("Failed to register new user with Username={0} and Registration token ={1}", _User.Username, _User.Token), ex.Message, UserStatus.Error.ToString());
            }
            return _id;
        }

        public JObject GetRegisteration(string Token, int ParentId = 0)
        {
            JObject _user = null;
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetRegisterationDetailsByToken", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = Token;
                        command.Parameters.Add("@ParentId", SqlDbType.Int).Value = ParentId;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            _Status = (int)Status.Value;
                            if (_Status == 1)
                            {
                                if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                {
                                    string RegisterType = String.Empty;
                                    RegisterType = _DataSet.Tables[0].Rows[0]["Type"].ToString();

                                    if (RegisterType.ToLower() != "schoolregistration")
                                    {
                                        var IsTrashed = bool.Parse(_DataSet.Tables[0].Rows[0]["IsTrashed"].ToString());
                                        var acStatus = bool.Parse(_DataSet.Tables[0].Rows[0]["Status"].ToString());
                                        if (acStatus == false && IsTrashed == true)
                                        {
                                            _user = JObject.FromObject(new UserBase()
                                            {
                                                returnStatus = (int)UserStatus.AccountIsDisabled
                                            });
                                            return _user;
                                        }
                                    }
                                    _user = JObject.FromObject(new UserBase()
                                    {
                                        Email = _DataSet.Tables[0].Rows[0]["Email"].ToString(),
                                        UserId = int.Parse(_DataSet.Tables[0].Rows[0]["UserId"].ToString()),
                                        MobileNumber = _DataSet.Tables[0].Rows[0]["MobileNo"].ToString(),
                                        Type = _DataSet.Tables[0].Rows[0]["Type"].ToString(),
                                        returnStatus = (int)UserStatus.Sucess
                                    });
                                }
                            }
                            else if (_Status == 0)
                            {
                                _user = JObject.FromObject(new UserBase()
                                {
                                    returnStatus = (int)UserStatus.InvalidToken
                                });
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetRegisteration", string.Format("Error occured in getting user by token {0}", Token), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _user = JObject.FromObject(new UserBase()
                            {
                                returnStatus = (int)UserStatus.Error
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetRegisteration", string.Format("Error occured in getting user by token {0}", Token), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _user = JObject.FromObject(new UserBase()
                {
                    returnStatus = (int)UserStatus.Error
                });
            }
            return _user;
        }

        public int VerifyMobile(string Email, string MobileNo, string Type, int EntityId)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spVerifyMobileNo", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                        command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = MobileNo;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                        command.Parameters.Add("@EntityId", SqlDbType.Int).Value = EntityId;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;
                        return _Status;
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "VerifyMobile", string.Format("Error occured in verifying mobile= {0}, email= {1}, usertype= {2}", MobileNo, Email, Type), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return 0;
            }
        }

        public DataSet GetUserByUserName(string Username)
        {
            DataSet ds = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUserByUsername", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = Username;
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetUserByUserName", string.Format("Error occured in getting user by username= {0}", Username), ex.Message.ToString(), UserStatus.Error.ToString());
                throw ex;
            }
        }

        public DataSet GetUserById(int UserId)
        {
            DataSet ds = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUserById", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetUserById", "Error occured while getting user by id " + ex.Message.ToString(), ex.Message.ToString(), UserStatus.Error.ToString());
                throw ex;
            }
        }

        public UserResult SendPasswordRecovery(int UserId, string Url)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spInsertUserToken", con))
                    {
                        string _Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = _Token;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = "passwordrecovery";

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            User _User = new User();
                            _User.Token = _Token;
                            InsertLog(UserId, "SendPasswordRecovery", "Send Password Recovery", string.Format("Password Recovery for {0}", UserId), UserStatus.Sucess.ToString());
                            return new UserResult { User = _User, Status = UserStatus.Sucess };
                        }
                        else if (Status == 2)
                        {
                            InsertLog(UserId, "SendPasswordRecovery", "Send Password Recovery", string.Format("No user exists with userid {0}", UserId), UserStatus.UserAccountNotExist.ToString());
                            return new UserResult { Status = UserStatus.UserAccountNotExist };
                        }
                        else
                        {
                            InsertLog(UserId, "SendPasswordRecovery", "Send Password Recovery", string.Format("Failed to send password recovery for {0}", UserId), UserStatus.UserAccountNotExist.ToString());
                            return new UserResult { Status = UserStatus.Error };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(UserId, "SendPasswordRecovery", string.Format("Error occured while sending password recovery for {0}", UserId), ex.InnerException.ToString(), UserStatus.UserAccountNotExist.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
        }

        public GenericStatus ValidateUserToken(string Token, string Type)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spValidateUserToken", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = Token;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type.ToLower();
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 0)
                        {
                            return GenericStatus.Error;
                        }
                        else if (Status > 0)
                        {
                            return GenericStatus.Sucess;
                        }
                        else
                        {
                            return GenericStatus.Other;
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "ValidateUserToken", string.Format("Error occured in validating user token={0}, type= {1}", Token, Type), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return GenericStatus.Error;
        }

        public UserStatus PasswordRecoverySubmit(string Token, string _Password, string PasswordSalt)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spRecoverPassword", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = Token;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = "passwordrecovery";
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = _Password;
                        command.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar).Value = PasswordSalt;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(0, "PasswordRecoverySubmit", "Password Recovery Submit", string.Format("Password recovery for token {0}", Token), UserStatus.Sucess.ToString());
                            return UserStatus.Sucess;
                        }
                        else if (Status == 0)
                        {
                            InsertLog(0, "PasswordRecoverySubmit", "Password Recovery Submit", string.Format("Invalid token {0}", Token), UserStatus.InvalidToken.ToString());
                            return UserStatus.InvalidToken;
                        }
                        else if (Status == 2)
                        {
                            InsertLog(0, "PasswordRecoverySubmit", "Password Recovery Submit", string.Format("User Account Not Exist with token {0}", Token), UserStatus.UserAccountNotExist.ToString());
                            return UserStatus.UserAccountNotExist;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(0, "PasswordRecoverySubmit", "Password Recovery Submit", "Error occured while performingPassword Recovery Submit", UserStatus.Error.ToString());                
                throw ex;
            }
            return UserStatus.Error;
        }

        public int ChangePassword(int UserId, string _Password, string PasswordSalt)
        {
            int Status = 0;
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdatePassword", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = _Password;
                        command.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar).Value = PasswordSalt;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                    }
                }
                InsertLog(_currentUserId, "ChangePassword", "Change Password", string.Format("Password changed for User {0}", UserId), UserStatus.Sucess.ToString());
                return Status;
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "ChangePassword", string.Format("Error occured while changing password for User {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                throw ex;
            }
        }

        public UserStatus UpdateUserProfile(UserProfile _User)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateUser", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = _User.User.UserId;
                        command.Parameters.Add("@AddressLine1", SqlDbType.NVarChar).Value = _User.User.AddressLine1;
                        command.Parameters.Add("@AddressLine2", SqlDbType.NVarChar).Value = _User.User.AddressLine2;
                        command.Parameters.Add("@State", SqlDbType.NVarChar).Value = _User.User.State;
                        command.Parameters.Add("@Country", SqlDbType.NVarChar).Value = _User.User.Country;
                        command.Parameters.Add("@PinCode", SqlDbType.NVarChar).Value = _User.User.PinCode;
                        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = _User.User.PhoneNumber;
                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_User.User.FirstName);
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_User.User.LastName);
                        command.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = _User.User.Gender;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = _User.User.Email;
                        command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = _User.User.DateOfBirth;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = _User.User.City;
                        command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = _User.User.MobileNumber;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(_currentUserId, "UpdateUserProfile", "Update user profile", string.Format("Update user with First Name={0}, Last Name={1}, Mobile={2}, Email={3}", _User.User.FirstName, _User.User.LastName, _User.User.MobileNumber, _User.User.Email), UserStatus.Sucess.ToString());
                            return UserStatus.Sucess;
                        }
                        else if (Status == 2)
                        {
                            InsertLog(_currentUserId, "UpdateUserProfile", "Update user profile", string.Format("Update user with First Name={0}, Last Name={1}, Mobile={2}, Email={3}", _User.User.FirstName, _User.User.LastName, _User.User.MobileNumber, _User.User.Email), UserStatus.UserAccountNotExist.ToString());
                            return UserStatus.UserAccountNotExist;
                        }
                        else if (Status == 3)
                        {
                            InsertLog(_currentUserId, "UpdateUserProfile", "Update user profile", string.Format("Update user with First Name={0}, Last Name={1}, Mobile={2}, Email={3}", _User.User.FirstName, _User.User.LastName, _User.User.MobileNumber, _User.User.Email), UserStatus.UserAlreadyRegistered.ToString());
                            return UserStatus.UserAlreadyRegistered;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UpdateUserProfile", string.Format("Failed to update user profile {0}", _User.User.UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return UserStatus.Error;
            }
            return UserStatus.Error;
        }

        public SchoolStatus CreateSchool(School _School)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spCreateSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.SchoolName);
                        command.Parameters.Add("@PrincipalName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.PrincipalName);
                        command.Parameters.Add("@PrincipalEmail", SqlDbType.NVarChar).Value = _School.PrincipalEmail.ToString().ToLower();
                        command.Parameters.Add("@AddressLine1", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.AddressLine1);
                        command.Parameters.Add("@AddressLine2", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.AddressLine2);
                        command.Parameters.Add("@State", SqlDbType.NVarChar).Value = _School.State;
                        command.Parameters.Add("@Country", SqlDbType.NVarChar).Value = _School.Country;
                        command.Parameters.Add("@PinCode", SqlDbType.NVarChar).Value = _School.PinCode;
                        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = _School.PhoneNumber;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = _School.City;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(_currentUserId, "CreateSchool", "New School Created Successfully", "New School Created Successfully with name " + _School.SchoolName, UserStatus.Sucess.ToString());
                            return SchoolStatus.Sucess;
                        }
                        else if (Status == 2)
                        {
                            InsertLog(_currentUserId, "CreateSchool", "School already exist", "School already exist with name " + _School.SchoolName, SchoolStatus.AlreadyHaveSchool.ToString());
                            return SchoolStatus.AlreadyHaveSchool;
                        }
                        else if (Status == 3)
                        {
                            InsertLog(_currentUserId, "CreateSchool", "School with UID already exist", "School with UID already exist", SchoolStatus.AlreadyHaveSchoolUId.ToString());
                            return SchoolStatus.AlreadyHaveSchoolUId;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "CreateSchool", "Error occured while creating school with name " + _School.SchoolName, ex.Message.ToString(), UserStatus.Error.ToString());
                return SchoolStatus.Error;
            }
            return SchoolStatus.Error;
        }

        public SchoolResult GetSchoolProfileDetails(string SchoolUId)
        {
            SchoolResult _Result = new SchoolResult();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetSchoolProfileDetails", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                School _School = new School();

                                _School.SchoolId = int.Parse(_DataSet.Tables[0].Rows[0]["SchoolId"].ToString());
                                _School.SchoolUId = _DataSet.Tables[0].Rows[0]["SchoolUId"].ToString();
                                _School.SchoolName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["Name"].ToString());
                                _School.AddressLine1 = _DataSet.Tables[0].Rows[0]["AddressLine1"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["AddressLine1"].ToString()) : "";
                                _School.AddressLine2 = _DataSet.Tables[0].Rows[0]["AddressLine2"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["AddressLine2"].ToString()) : "";
                                _School.State = _DataSet.Tables[0].Rows[0]["State"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["State"].ToString() : "";
                                _School.Country = _DataSet.Tables[0].Rows[0]["Country"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["Country"].ToString() : "";
                                _School.PinCode = _DataSet.Tables[0].Rows[0]["PinCode"] != DBNull.Value ? int.Parse(_DataSet.Tables[0].Rows[0]["PinCode"].ToString()) : 0;
                                _School.PrincipalName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["PrincipalName"].ToString());
                                _School.PrincipalEmail = _DataSet.Tables[0].Rows[0]["PrincipalEmailID"].ToString();
                                _School.PhoneNumber = _DataSet.Tables[0].Rows[0]["PhoneNumber"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["PhoneNumber"].ToString() : "";
                                _School.IsActive = bool.Parse(_DataSet.Tables[0].Rows[0]["IsActive"].ToString());
                                _School.IsTrashed = bool.Parse(_DataSet.Tables[0].Rows[0]["IsTrashed"].ToString());
                                _School.IsEmailVerified = bool.Parse(_DataSet.Tables[0].Rows[0]["IsEmailVerified"].ToString());
                                _School.City = _DataSet.Tables[0].Rows[0]["City"].ToString();
                                _Result.MySchool = _School;
                                _Result.Status = SchoolStatus.Sucess;
                            }
                            else
                            {
                                _Result.Status = SchoolStatus.Error;
                            }

                        }
                        catch
                        {
                            _Result.Status = SchoolStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetSchoolProfileDetails", "Error Occured while getting School Profile Details with UID=" + SchoolUId, ex.Message.ToString(), UserStatus.Error.ToString());
                _Result.Status = SchoolStatus.Error;
            }
            return _Result;
        }

        public SchoolData GetSchoolByUId(string SchoolUId, int PageIndex, int PageSize)
        {
            SchoolData _Result = new SchoolData();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        SqlParameter id = new SqlParameter("@StatusApi", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            int Status = (int)id.Value;
                            if (Status == 1)
                            {
                                if (_DataSet != null)
                                {
                                    if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                    {
                                        _Result.SchoolDetails.SchoolId = int.Parse(_DataSet.Tables[0].Rows[0]["SchoolId"].ToString());
                                        _Result.SchoolDetails.SchoolUId = _DataSet.Tables[0].Rows[0]["SchoolUId"].ToString();
                                        _Result.SchoolDetails.SchoolName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["Name"].ToString());
                                        _Result.SchoolDetails.AddressLine1 = _DataSet.Tables[0].Rows[0]["AddressLine1"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["AddressLine1"].ToString()) : "";
                                        _Result.SchoolDetails.AddressLine2 = _DataSet.Tables[0].Rows[0]["AddressLine2"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["AddressLine2"].ToString()) : "";
                                        _Result.SchoolDetails.State = _DataSet.Tables[0].Rows[0]["State"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["State"].ToString() : "";
                                        _Result.SchoolDetails.Country = _DataSet.Tables[0].Rows[0]["Country"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["Country"].ToString() : "";
                                        _Result.SchoolDetails.PinCode = _DataSet.Tables[0].Rows[0]["PinCode"] != DBNull.Value ? int.Parse(_DataSet.Tables[0].Rows[0]["PinCode"].ToString()) : 0;
                                        _Result.SchoolDetails.PrincipalName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["PrincipalName"].ToString());
                                        _Result.SchoolDetails.PrincipalEmail = _DataSet.Tables[0].Rows[0]["PrincipalEmailID"].ToString();
                                        _Result.SchoolDetails.PhoneNumber = _DataSet.Tables[0].Rows[0]["PhoneNumber"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["PhoneNumber"].ToString() : "";
                                        _Result.SchoolDetails.IsActive = bool.Parse(_DataSet.Tables[0].Rows[0]["IsActive"].ToString());
                                        _Result.SchoolDetails.StudentCount = _DataSet.Tables[0].Rows[0]["StudentCount"] != DBNull.Value ? int.Parse(_DataSet.Tables[0].Rows[0]["StudentCount"].ToString()) : 0;
                                        _Result.SchoolDetails.SchoolAdminCount = _DataSet.Tables[0].Rows[0]["SchoolAdminCount"] != DBNull.Value ? int.Parse(_DataSet.Tables[0].Rows[0]["SchoolAdminCount"].ToString()) : 0;
                                        _Result.SchoolDetails.City = _DataSet.Tables[0].Rows[0]["City"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["City"].ToString() : "";
                                        _Result.SchoolDetails.IsEmailVerified = _DataSet.Tables[0].Rows[0]["IsEmailVerified"] != DBNull.Value ? Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["IsEmailVerified"]) : false;
                                        _Result.SchoolDetails.IsTrashed = bool.Parse(_DataSet.Tables[0].Rows[0]["IsTrashed"].ToString());
                                    }
                                    if (_DataSet.Tables[1] != null && _DataSet.Tables[1].Rows.Count != 0)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                        {
                                            UserCommon _User = new UserCommon();
                                            _User.UserId = int.Parse(_DataRow["UserId"].ToString());
                                            _User.Email = _DataRow["Email"].ToString();
                                            _User.MobileNumber = _DataRow["MobileNo"].ToString();
                                            _User.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                            _User.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                            _User.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                            _User.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                            _User.Status = bool.Parse(_DataRow["Status"].ToString());
                                            _User.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                            _User.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                            _Result.Admins.Items.Add(_User);
                                        }
                                        _Result.Admins.TotalItems = int.Parse(_DataSet.Tables[1].Rows[0]["TotalRows"].ToString());
                                        _Result.Admins.PageSize = PageSize;
                                    }
                                    if (_DataSet.Tables[2] != null && _DataSet.Tables[2].Rows.Count != 0)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                        {
                                            StudentModel _Student = new StudentModel();
                                            _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                            _Student.Email = _DataRow["Email"].ToString();
                                            _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                            _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                            _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                            _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                            _Student.ParentEmail = _DataRow["ParentEmail"].ToString();
                                            _Student.Grade = _DataRow["GradeName"].ToString();
                                            _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                            _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                            _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                            _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                            _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                            _Result.Students.Items.Add(_Student);
                                        }
                                        _Result.Students.TotalItems = int.Parse(_DataSet.Tables[2].Rows[0]["TotalRows"].ToString());
                                        _Result.Students.PageSize = PageSize;
                                    }

                                    if (_DataSet.Tables.Count > 3)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                        {
                                            Grade _Grade = new Grade();
                                            _Grade.Id = int.Parse(_DataRow["Id"].ToString());
                                            _Grade.Name = _DataRow["Name"].ToString();
                                            _Result.Grades.Add(_Grade);
                                        }
                                    }
                                }
                                _Result.APIStatus = SchoolStatus.Sucess;
                            }
                            else
                                _Result.APIStatus = SchoolStatus.NoSchoolFound;
                        }
                        catch { }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetSchoolByUId", string.Format("Error occured while getting school by UId {0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());

            }
            return _Result;
        }

        public SchoolStatus UpdateSchool(School _School)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = _School.SchoolId;
                        command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.SchoolName);
                        command.Parameters.Add("@PrincipalName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.PrincipalName);
                        command.Parameters.Add("@PrincipalEmail", SqlDbType.NVarChar).Value = _School.PrincipalEmail;
                        command.Parameters.Add("@AddressLine1", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.AddressLine1);
                        command.Parameters.Add("@AddressLine2", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_School.AddressLine2);
                        command.Parameters.Add("@State", SqlDbType.NVarChar).Value = _School.State;
                        command.Parameters.Add("@Country", SqlDbType.NVarChar).Value = _School.Country;
                        command.Parameters.Add("@PinCode", SqlDbType.NVarChar).Value = _School.PinCode;
                        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = _School.PhoneNumber;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = _School.City;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(_currentUserId, "UpdateSchool", "Update School", string.Format("{0} school updated", _School.SchoolName), SchoolStatus.Sucess.ToString());
                            return SchoolStatus.Sucess;
                        }
                        else if (Status == 2)
                        {
                            InsertLog(_currentUserId, "UpdateSchool", "Update School", string.Format("No school found to update with name {0}", _School.SchoolName), SchoolStatus.NoSchoolFound.ToString());
                            return SchoolStatus.NoSchoolFound;
                        }
                        else if (Status == 3)
                        {
                            InsertLog(_currentUserId, "UpdateSchool", "Update School", string.Format("{0} school already exists in {1} pincode", _School.SchoolName, _School.PinCode), SchoolStatus.AlreadyHaveSchool.ToString());
                            return SchoolStatus.AlreadyHaveSchool;
                        }
                        else if (Status == 4)
                        {
                            InsertLog(_currentUserId, "UpdateSchool", "Update School", string.Format("{0} SchoolUId already exists", _School.SchoolUId), SchoolStatus.AlreadyHaveSchoolUId.ToString());
                            return SchoolStatus.AlreadyHaveSchoolUId;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UpdateSchool", string.Format("Error occured while updating school {0}", _School.SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolStatus.Error;
                //throw ex;
            }

            return SchoolStatus.Error;
        }

        public PagedList<UserCommon> GetSchoolAdmins(string SchoolUId, int PageIndex, int PageSize, string SearchTxt)
        {
            PagedList<UserCommon> _Result = new PagedList<UserCommon>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetSchoolAdmins", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    UserCommon _User = new UserCommon();
                                    _User.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _User.Username = _DataRow["UserName"].ToString();
                                    _User.Email = _DataRow["Email"].ToString();
                                    _User.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _User.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _User.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _User.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _User.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _User.Status = bool.Parse(_DataRow["Status"].ToString());
                                    _User.Gender = _DataRow["Gender"].ToString();
                                    _User.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _User.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Items.Add(_User);
                                }
                                _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                _Result.PageSize = PageSize;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetSchoolAdmins", string.Format("Error occured while getting school admins for school {}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetSchoolAdmins", string.Format("Error occured while getting school admins for school {}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public StudentModel GetSchoolAdmin(string SchoolUId, int UserId)
        {
            StudentModel _Result = new StudentModel();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetSchoolAdmin", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _Result.Email = _DataRow["Email"].ToString();
                                    _Result.Username = _DataRow["UserName"].ToString();
                                    _Result.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _Result.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Result.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Result.DateOfBirth = _DataRow["DateOfBirth"] != DBNull.Value ? DateTime.Parse(_DataRow["DateOfBirth"].ToString()) : (DateTime?)null;
                                    _Result.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Result.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _Result.Gender = _DataRow["Gender"].ToString();
                                    _Result.Status = bool.Parse(_DataRow["Status"].ToString());
                                    _Result.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _Result.AddressLine1 = _DataSet.Tables[0].Rows[0]["AddressLine1"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["AddressLine1"].ToString() : "";
                                    _Result.AddressLine2 = _DataSet.Tables[0].Rows[0]["AddressLine2"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["AddressLine2"].ToString() : "";
                                    _Result.State = _DataSet.Tables[0].Rows[0]["State"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["State"].ToString() : "";
                                    _Result.Country = _DataSet.Tables[0].Rows[0]["Country"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["Country"].ToString() : "";
                                    _Result.PinCode = _DataSet.Tables[0].Rows[0]["PinCode"] != DBNull.Value ? int.Parse(_DataSet.Tables[0].Rows[0]["PinCode"].ToString()) : 0;
                                    _Result.PhoneNumber = _DataSet.Tables[0].Rows[0]["PhoneNumber"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["PhoneNumber"].ToString() : "";
                                    _Result.SchoolName = _DataSet.Tables[0].Rows[0]["SchoolName"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["SchoolName"].ToString()) : "";
                                    _Result.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _Result.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                }

                            }
                            else
                            {

                                _Result = null;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetSchoolAdmin", string.Format("Error occured in getting school admin Id= {0}, schoolUid= {1}", UserId, SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetSchoolAdmin", string.Format("Error occured in getting school admin Id= {0}, schoolUid= {1}", UserId, SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public UserResult AddSchoolAdmins(string SchoolUId, string Email, string MobileNo)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spAddSchoolAdmin", con))
                    {
                        string Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@SchoolUId", SqlDbType.NVarChar).Value = SchoolUId;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                        command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = MobileNo;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = Token;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;

                        if (_Status == 1)
                        {
                            User _User = new User();
                            _User.Token = Token;
                            InsertLog(_currentUserId, "AddSchoolAdmins", "Add new school admin", string.Format("New school admin added for school {0}, with email {1}", SchoolUId, Email), UserStatus.Sucess.ToString());
                            return new UserResult { User = _User, Status = UserStatus.Sucess };
                        }
                        else if (_Status == 0)
                        {
                            InsertLog(_currentUserId, "AddSchoolAdmins", "Add new school admin", string.Format("User already exists with email {0}", Email), UserStatus.UserAlreadyRegistered.ToString());
                            return new UserResult { Status = UserStatus.UserAlreadyRegistered };
                        }
                        else if (_Status == -1)
                        {
                            InsertLog(_currentUserId, "AddSchoolAdmins", "Add new school admin", string.Format("Failed to add new school admin for school {0}, with email {1}", SchoolUId, Email), UserStatus.Error.ToString());
                            return new UserResult { Status = UserStatus.Error };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "AddSchoolAdmins", string.Format("Error occured while adding new school admin for school {0}, with email {1}", SchoolUId, Email), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
            return new UserResult { Status = UserStatus.Error };
        }

        public PagedList<School> GetSchoolList(int PageIndex, int PageSize, string SearchTxt)
        {
            PagedList<School> _Result = new PagedList<School>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetSchoolList", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    School _School = new School();
                                    _School.SchoolId = int.Parse(_DataRow["SchoolId"].ToString());
                                    _School.SchoolUId = _DataRow["SchoolUId"].ToString();
                                    _School.SchoolName = HttpUtility.HtmlDecode(_DataRow["Name"].ToString());
                                    _School.PrincipalName = HttpUtility.HtmlDecode(_DataRow["PrincipalName"].ToString());
                                    _School.PrincipalEmail = _DataRow["PrincipalEmailID"].ToString();
                                    _School.IsActive = bool.Parse(_DataRow["IsActive"].ToString());
                                    _School.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _School.StudentCount = _DataRow["StudentCount"] != DBNull.Value ? int.Parse(_DataRow["StudentCount"].ToString()) : 0;
                                    _School.SchoolAdminCount = _DataRow["SchoolAdminCount"] != DBNull.Value ? int.Parse(_DataRow["SchoolAdminCount"].ToString()) : 0;
                                    _School.AddressLine1 = _DataRow["AddressLine1"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataRow["AddressLine1"].ToString()) : "";
                                    _School.AddressLine2 = _DataRow["AddressLine2"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataRow["AddressLine2"].ToString()) : "";
                                    _School.State = _DataRow["State"] != DBNull.Value ? _DataRow["State"].ToString() : "";
                                    _School.Country = _DataRow["Country"] != DBNull.Value ? _DataRow["Country"].ToString() : "";
                                    _School.PinCode = _DataRow["PinCode"] != DBNull.Value ? int.Parse(_DataRow["PinCode"].ToString()) : 0;
                                    _School.City = _DataRow["City"] != DBNull.Value ? _DataRow["City"].ToString() : "";
                                    _School.IsEmailVerified = _DataRow["IsEmailVerified"] != DBNull.Value ? bool.Parse(_DataRow["IsEmailVerified"].ToString()) : false;

                                    _Result.Items.Add(_School);
                                }
                                _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                _Result.PageSize = PageSize;

                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetSchoolList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetSchoolList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }
        public string CheckDevice(string DeviceDetails)
        {

            string _deviceEnvironment = "";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spCheckDevice", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@DeviceDetails", SqlDbType.NVarChar).Value = DeviceDetails;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                _deviceEnvironment = _DataSet.Tables[0].Rows[0]["Environment"].ToString();
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "CheckDevice", string.Format("Error occured in device checking with DeviceDetails {0}", DeviceDetails), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            return "";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "CheckDevice", string.Format("Error occured in device checking with DeviceDetails {0}", DeviceDetails), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return "";
            }
            return _deviceEnvironment;
        }
        public List<Avatar> GetAvatar(int UserId)
        {
            List<Avatar> _ADetails = new List<Avatar>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetAvatar", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);


                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    Avatar _AvatarDetail = new Avatar();

                                    _AvatarDetail.AvatarId = _DataRow["Id"] != DBNull.Value ? int.Parse(_DataRow["Id"].ToString()) : 0;
                                    _AvatarDetail.Name = _DataRow["Name"].ToString();
                                    _AvatarDetail.ImagePath = _DataRow["ImagePath"].ToString();
                                    _ADetails.Add(_AvatarDetail);

                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetAvatar", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetAvatar", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _ADetails;
        }
        public List<UserEvent> GetEvents(int UserId)
        {
            List<UserEvent> _Details = new List<UserEvent>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetEvents", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);


                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    UserEvent _EventsDetail = new UserEvent();

                                    _EventsDetail.EventId = int.Parse(_DataRow["Id"].ToString());
                                    _EventsDetail.Name = _DataRow["Name"].ToString();
                                    _EventsDetail.Description = _DataRow["Description"].ToString();
                                    _EventsDetail.StartDate = DateTime.Parse(_DataRow["StartDate"].ToString());
                                    _EventsDetail.EndDate = DateTime.Parse(_DataRow["EndDate"].ToString());
                                    _EventsDetail.IsView = bool.Parse(_DataRow["IsRead"].ToString());
                                    _Details.Add(_EventsDetail);
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetEvents", string.Format("Error occured in geeting events for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Details = null;

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetEvents", string.Format("Error occured in geeting events for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Details = null;

            }
            return _Details;
        }
        public GenericStatus UpdateViewEvents(string EventIds, int UserId)
        {
            int _Status = 0;
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateEvents", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@EventIds", SqlDbType.NVarChar).Value = EventIds;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;
                        if (_Status == 1)
                        {
                            InsertLog(_currentUserId, "UpdateViewEvents", "Update view events", string.Format("Event {0}, updated for user {1}", EventIds, UserId), UserStatus.Sucess.ToString());
                            return GenericStatus.Sucess;
                        }
                        else
                        {
                            InsertLog(_currentUserId, "UpdateViewEvents", "Update view events", string.Format("Failed to update event {0}, for user {1}", EventIds, UserId), UserStatus.Error.ToString());
                            return GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UpdateViewEvents", string.Format("Error occured while updating event {0}, for user {1}", EventIds, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }

            return GenericStatus.Error;
        }

        public AddDeviceModel AddDevice(DeviceDetail _Device)
        {
            int Status = 0;
            try
            {
                if (_Device.UserId > 0)
                    _currentUserId = _Device.UserId;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spAddDevice", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@DeviceDetails", SqlDbType.NVarChar).Value = _Device.DeviceDetails;
                        command.Parameters.Add("@DeviceName", SqlDbType.NVarChar).Value = _Device.DeviceName;
                        command.Parameters.Add("@DeviceOS", SqlDbType.NVarChar).Value = _Device.DeviceOS;
                        command.Parameters.Add("@Environment", SqlDbType.NVarChar).Value = _Device.Environment;
                        command.Parameters.Add("@Platform", SqlDbType.NVarChar).Value = _Device.Platform;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = _Device.UserId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (_Device.Environment.ToLower() == "home")
                        {
                            SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                            DataSet _DataSet = new DataSet();
                            _SqlDataAdapter.Fill(_DataSet);
                            Status = (int)id.Value;
                            if (Status > 0)
                            {
                                return new AddDeviceModel() { Status = DeviceStatus.Sucess, DeviceId = Status };
                            }
                            else if (Status == -1)
                            {
                                if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                {
                                    AddDeviceModel _Result = new AddDeviceModel() { Status = DeviceStatus.DeviceAlreadyHave };
                                    _Result.Devices = new List<DeviceDetail>();
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        DeviceDetail _DeviceDetail = new DeviceDetail();
                                        _DeviceDetail.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _DeviceDetail.DeviceId = int.Parse(_DataRow["DeviceId"].ToString());
                                        _DeviceDetail.DeviceDetails = _DataRow["DeviceDetails"].ToString();
                                        _DeviceDetail.DeviceName = _DataRow["DeviceName"].ToString();
                                        _DeviceDetail.DeviceOS = _DataRow["DeviceOS"].ToString();
                                        _DeviceDetail.Environment = _DataRow["Environment"].ToString();
                                        _DeviceDetail.AddedOn = _DataRow["AddedOn"] != DBNull.Value ? DateTime.Parse(_DataRow["AddedOn"].ToString()) : (DateTime?)null;
                                        _Result.Devices.Add(_DeviceDetail);
                                    }
                                    InsertLog(_currentUserId, "AddDevice", "add home device", string.Format("New device {0}, is added for user {1}", _Device.DeviceDetails, _Device.UserId), UserStatus.Sucess.ToString());
                                    return _Result;
                                }

                            }
                            else
                            {
                                InsertLog(_currentUserId, "AddDevice", "add home device", string.Format("Failed to add new device {0}, for user {1}", _Device.DeviceDetails, _Device.UserId), UserStatus.Error.ToString());
                                return new AddDeviceModel() { Status = DeviceStatus.Error };
                            }
                        }
                        else
                        {
                            if (con.State == ConnectionState.Open)
                                con.Close();
                            con.Open();
                            command.ExecuteNonQuery();
                            Status = (int)id.Value;
                            if (Status > 0)
                            {
                                InsertLog(_currentUserId, "AddDevice", "add school device", string.Format("New device {0}, is added for user {1}", _Device.DeviceDetails, _Device.UserId), UserStatus.Sucess.ToString());
                                return new AddDeviceModel() { Status = DeviceStatus.Sucess, DeviceId = Status };
                            }
                            else
                            {
                                InsertLog(_currentUserId, "AddDevice", "add school device", string.Format("Failed to add new device {0}, for user {1}", _Device.DeviceDetails, _Device.UserId), UserStatus.Error.ToString());
                                return new AddDeviceModel() { Status = DeviceStatus.Error };
                            }

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "AddDevice", string.Format("Error occured adding new device {0}, for user {1}", _Device.DeviceDetails, _Device.UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new AddDeviceModel() { Status = DeviceStatus.Error };
            }
            return new AddDeviceModel() { Status = DeviceStatus.Error };
        }

        public bool AddEditAvatar(AddEditAvatar _Avatar)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spAddEditAvatar", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = _Avatar.UserId;
                        command.Parameters.Add("@AvatarId", SqlDbType.Int).Value = _Avatar.AvatarId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(_Avatar.UserId, "AddEditAvatar", "Add edit avatar", string.Format("avatar {0} is added to user {1}", _Avatar.AvatarId, _Avatar.UserId), UserStatus.Sucess.ToString());
                            return true;
                        }
                        else if (Status == 0)
                        {
                            InsertLog(_Avatar.UserId, "AddEditAvatar", "Add edit avatar", string.Format("Failed to add avatar {0} to user {1}", _Avatar.AvatarId, _Avatar.UserId), UserStatus.Error.ToString());
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "AddEditAvatar", string.Format("Error occured in adding avatar {0} to user {1}", _Avatar.AvatarId, _Avatar.UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
            return false;
        }

        public bool SaveRating(BookRating _Rating)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spInsertUpdateRating", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = _Rating.UserId;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = _Rating.BookId;
                        command.Parameters.Add("@Rating", SqlDbType.Int).Value = _Rating.Rating;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(_currentUserId, "SaveRating", "Save user book rating", string.Format("Rating {0} for book {1} is added by user {2}", _Rating.Rating, _Rating.BookId, _Rating.UserId), UserStatus.Sucess.ToString());
                            return true;
                        }
                        else if (Status == 0)
                        {
                            InsertLog(_currentUserId, "SaveRating", "Save user book rating", string.Format("Failed to add Rating {0} for book {1} by user {2}", _Rating.Rating, _Rating.BookId, _Rating.UserId), UserStatus.Error.ToString());
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SaveRating", string.Format("Error occured in adding Rating {0} for book {1} by user {2}", _Rating.Rating, _Rating.BookId, _Rating.UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
            return false;
        }


        public GenericStatus RemoveUsersFromSchool(string UserIds, int SchoolId)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spRemoveUsersFromSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserIds", SqlDbType.NVarChar).Value = UserIds;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;
                        if (_Status == 1)
                        {
                            InsertLog(_currentUserId, "RemoveUsersFromSchool", "", "", UserStatus.Sucess.ToString());
                            return GenericStatus.Sucess;
                        }
                        else
                        {
                            InsertLog(_currentUserId, "RemoveUsersFromSchool", "", "", UserStatus.Error.ToString());
                            return GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "RemoveUsersFromSchool", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }

            return GenericStatus.Error;
        }

        public PagedList<StudentModel> GetStudentsOfSchool(string SchoolUId, int PageSize, int PageIndex, string SearchTxt, string Grade)
        {
            int _Status = 0;
            PagedList<StudentModel> _Result = new PagedList<StudentModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudentsOfSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@Grade", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Grade) ? "" : Grade;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            _Status = (int)id.Value;
                            if (_Status == 1)
                            {
                                if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        StudentModel _Student = new StudentModel();
                                        _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Student.Email = _DataRow["Email"].ToString();
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.Username = _DataRow["UserName"] != DBNull.Value ? _DataRow["UserName"].ToString().ToLower() : "";
                                        _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                        _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                        _Student.ParentEmail = _DataRow["ParentEmail"].ToString();
                                        _Student.ParentMobileNo = _DataRow["ParentMobileNo"].ToString();
                                        _Student.Grade = _DataRow["GradeName"].ToString();
                                        _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                        _Student.Gender = _DataRow["Gender"].ToString();
                                        _Student.SubSection = HttpUtility.HtmlDecode(_DataRow["Section"].ToString());
                                        _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                        _Student.DateOfBirth = _DataRow["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(_DataRow["DateOfBirth"]) : (DateTime?)null;
                                        _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                        _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                        _Student.ParentUsername = _DataRow["ParentUsername"] != DBNull.Value ? _DataRow["ParentUsername"].ToString().ToLower() : "";
                                        _Student.ParentFirstname = _DataRow["ParentUsername"] != DBNull.Value ? _DataRow["ParentFirstName"].ToString().ToLower() : "";
                                        _Student.ParentLastname = _DataRow["ParentUsername"] != DBNull.Value ? _DataRow["ParentLastName"].ToString().ToLower() : "";
                                        _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                        _Result.Items.Add(_Student);
                                    }
                                    _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                    _Result.PageSize = PageSize;
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetStudentsOfSchool", string.Format("Error occured in getting students of school {0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetStudentsOfSchool", string.Format("Error occured in getting students of school {0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public StudentResult GetStudentOfSchool(int SchoolId, int UserId)
        {
            StudentResult _Result = new StudentResult();
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudentOfSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            _Status = (int)id.Value;
                            if (_Status == 1)
                            {
                                if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                {
                                    StudentModel _Student = new StudentModel();

                                    _Student.UserId = int.Parse(_DataSet.Tables[0].Rows[0]["UserId"].ToString());
                                    _Student.FirstName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["FirstName"].ToString());
                                    _Student.LastName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["LastName"].ToString());
                                    _Student.Gender = _DataSet.Tables[0].Rows[0]["Gender"].ToString();
                                    _Student.DateOfBirth = DateTime.Parse(_DataSet.Tables[0].Rows[0]["DateOfBirth"].ToString());
                                    _Student.Email = _DataSet.Tables[0].Rows[0]["Email"].ToString();
                                    _Student.RegistrationDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["RegistrationDate"].ToString());
                                    _Student.Grade = _DataSet.Tables[0].Rows[0]["StudentCount"].ToString();
                                    _Student.SubSection = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["SubSection"].ToString());
                                    _Student.RollNo = _DataSet.Tables[0].Rows[0]["RollNo"].ToString();
                                    _Student.SubscriptionStartDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["SubscriptionStartDate"].ToString());
                                    _Student.SubscriptionEndDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["SubscriptionEndDate"].ToString());
                                    _Result.Student = _Student;
                                    _Result.Status = UserStatus.Sucess;
                                }
                                else
                                {
                                    _Result.Status = UserStatus.Error;
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetStudentOfSchool", string.Format("Error occured in getting student {0} of school {1}", UserId, SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = UserStatus.Error;
                        }
                    }
                }
                return _Result;
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetStudentOfSchool", string.Format("Error occured in getting student {0} of school {1}", UserId, SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                throw ex;
            }
        }

        public PagedList<User> GetELibraryAdminsList(int PageIndex, int PageSize, string SearchTxt)
        {
            PagedList<User> _Result = new PagedList<User>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetELibraryAdminsList", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    User _User = new User();
                                    _User.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _User.Email = _DataRow["Email"].ToString();
                                    _User.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _User.Username = _DataRow["UserName"].ToString();
                                    _User.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _User.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _User.Status = Convert.ToBoolean(_DataRow["Status"]);
                                    _User.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                    _User.Gender = _DataRow["Gender"].ToString();
                                    _User.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _User.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _User.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Items.Add(_User);
                                }
                                _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                _Result.PageSize = PageSize;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetELibraryAdminsList", "Error occured in getting elib admins list", ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetELibraryAdminsList", "Error occured in getting elib admins list", ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public StudentParentResult GetStudentById(string SchoolUId, int UserId)
        {
            StudentParentResult _Result = new StudentParentResult();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudentById", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            int _Status = (int)Status.Value;
                            if (_DataSet.Tables.Count != 0 && _DataSet.Tables.Count > 2 && _Status == 1)
                            {
                                if (_DataSet.Tables[0].Rows.Count > 0)
                                {
                                    _Result.UserId = int.Parse(_DataSet.Tables[0].Rows[0]["UserId"].ToString());
                                    _Result.FirstName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["FirstName"].ToString());
                                    _Result.LastName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["LastName"].ToString());
                                    _Result.Grade = _DataSet.Tables[0].Rows[0]["Grade"].ToString();
                                    _Result.SubSection = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["SubSection"].ToString());
                                    _Result.RollNo = _DataSet.Tables[0].Rows[0]["RollNo"].ToString();
                                    _Result.Gender = _DataSet.Tables[0].Rows[0]["Gender"].ToString();
                                    _Result.DateOfBirth = _DataSet.Tables[0].Rows[0]["DateOfBirth"] != DBNull.Value ? DateTime.Parse(_DataSet.Tables[0].Rows[0]["DateOfBirth"].ToString()) : (DateTime?)null;
                                    _Result.CreationDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["CreationDate"].ToString());
                                    _Result.RegistrationDate = _DataSet.Tables[0].Rows[0]["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataSet.Tables[0].Rows[0]["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Result.Status = Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["Status"]);
                                    _Result.SubscriptionStartDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["SubscriptionStartDate"].ToString());
                                    _Result.SubscriptionEndDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["SubscriptionEndDate"].ToString());
                                    _Result.Username = _DataSet.Tables[0].Rows[0]["UserName"].ToString();
                                    _Result.HomeDevices = _DataSet.Tables[0].Rows[0]["HomeDevices"].ToString();
                                    _Result.SchoolUId = _DataSet.Tables[0].Rows[0]["SchoolUId"].ToString();
                                    _Result.SchoolStatus = Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["SchoolStatus"]);
                                    _Result.IsTrashed = Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["IsTrashed"]);
                                    _Result.SchoolName = HttpUtility.HtmlDecode(Convert.ToString(_DataSet.Tables[0].Rows[0]["Name"]));
                                    _Result.LastLoginDate = _DataSet.Tables[0].Rows[0]["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataSet.Tables[0].Rows[0]["LastLoginDate"].ToString()) : (DateTime?)null;
                                }

                                if (_DataSet.Tables[1].Rows.Count > 0)
                                {
                                    _Result.ParentDetails.UserId = int.Parse(_DataSet.Tables[1].Rows[0]["UserId"].ToString());
                                    _Result.ParentDetails.FirstName = HttpUtility.HtmlDecode(_DataSet.Tables[1].Rows[0]["FirstName"].ToString());
                                    _Result.ParentDetails.LastName = HttpUtility.HtmlDecode(_DataSet.Tables[1].Rows[0]["LastName"].ToString());
                                    _Result.ParentDetails.MobileNumber = _DataSet.Tables[1].Rows[0]["MobileNo"].ToString();
                                    _Result.ParentDetails.Email = _DataSet.Tables[1].Rows[0]["Email"].ToString();
                                    _Result.ParentDetails.Gender = _DataSet.Tables[1].Rows[0]["Gender"].ToString();
                                    _Result.ParentDetails.AddressLine1 = _DataSet.Tables[1].Rows[0]["AddressLine1"].ToString();
                                    _Result.ParentDetails.AddressLine2 = _DataSet.Tables[1].Rows[0]["AddressLine2"].ToString();
                                    _Result.ParentDetails.City = _DataSet.Tables[1].Rows[0]["City"].ToString();
                                    _Result.ParentDetails.State = _DataSet.Tables[1].Rows[0]["State"].ToString();
                                    _Result.ParentDetails.Country = _DataSet.Tables[1].Rows[0]["Country"].ToString();
                                    _Result.ParentDetails.PinCode = _DataSet.Tables[1].Rows[0]["PinCode"] != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Rows[0]["PinCode"]) : 0;
                                    _Result.ParentDetails.CreationDate = DateTime.Parse(_DataSet.Tables[1].Rows[0]["CreationDate"].ToString());
                                    _Result.ParentDetails.RegistrationDate = _DataSet.Tables[1].Rows[0]["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataSet.Tables[1].Rows[0]["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Result.ParentDetails.Status = Convert.ToBoolean(_DataSet.Tables[1].Rows[0]["Status"]);
                                    _Result.ParentDetails.Username = _DataSet.Tables[1].Rows[0]["UserName"].ToString();
                                    _Result.ParentDetails.IsTrashed = Convert.ToBoolean(_DataSet.Tables[1].Rows[0]["IsTrashed"]);
                                    _Result.ParentDetails.LastLoginDate = _DataSet.Tables[1].Rows[0]["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataSet.Tables[1].Rows[0]["LastLoginDate"].ToString()) : (DateTime?)null;
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    StudentModel OtherStudent = new StudentModel();
                                    OtherStudent.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    OtherStudent.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    OtherStudent.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    OtherStudent.Email = _DataRow["Email"].ToString();
                                    OtherStudent.SubscriptionStartDate = DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString());
                                    OtherStudent.SubscriptionEndDate = DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString());
                                    OtherStudent.SchoolId = Convert.ToInt32(_DataRow["SchoolId"]);
                                    OtherStudent.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                    OtherStudent.Status = Convert.ToBoolean(_DataRow["Status"]);
                                    OtherStudent.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Students.Add(OtherStudent);
                                }

                                _Result.APIStatus = UserStatus.Sucess;
                            }
                            else
                            {
                                _Result.APIStatus = UserStatus.UserAccountNotExist;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetStudentById", string.Format("Error occured in getting student {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.APIStatus = UserStatus.Error;
                        }
                    }
                }
                return _Result;
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetStudentById", string.Format("Error occured in getting student {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                throw ex;
            }
        }

        public BooksListResult GetBooksListWithFilter(int PageIndex, int PageSize)
        {
            BooksListResult _Result = new BooksListResult();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBooksListWithFilter", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count > 3)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    SubSection _Section = new SubSection();
                                    _Section.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Section.Name = _DataRow["Name"].ToString();
                                    _Result.SubSection.Add(_Section);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    Language _Language = new Language();
                                    _Language.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Language.Name = _DataRow["Name"].ToString();
                                    _Result.Language.Add(_Language);
                                }
                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    BookType _Type = new BookType();
                                    _Type.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Type.Name = _DataRow["Type"].ToString();
                                    _Result.BookType.Add(_Type);
                                }
                                if (_DataSet.Tables[3].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                    {
                                        Book _Book = new Book();
                                        _Book.BookId = int.Parse(_DataRow["BookId"].ToString());
                                        _Book.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString());
                                        _Book.KitabletID = _DataRow["KitabletID"].ToString();
                                        _Book.Thumbnail1 = _DataRow["Thumbnail1"].ToString();
                                        _Book.Thumbnail2 = _DataRow["Thumbnail2"].ToString();
                                        _Book.Thumbnail3 = _DataRow["Thumbnail3"].ToString();
                                        _Book.HasActivity = Convert.ToBoolean(_DataRow["HasActivity"]);
                                        _Book.HasReadAloud = Convert.ToBoolean(_DataRow["HasReadAloud"]);
                                        _Book.HasAnimation = Convert.ToBoolean(_DataRow["HasAnimation"]);
                                        _Book.ViewMode = _DataRow["ViewMode"].ToString();
                                        _Book.SubSection = _DataRow["SubSection"].ToString();
                                        _Book.Rating = _DataRow["Rating"] != DBNull.Value ? Convert.ToSingle(_DataRow["Rating"]) : 0;
                                        _Result.Books.Items.Add(_Book);
                                    }

                                    _Result.Books.TotalItems = int.Parse(_DataSet.Tables[3].Rows[0]["TotalRows"].ToString());
                                    _Result.Books.PageSize = PageSize;
                                    _Result.Books.PageIndex = PageIndex;
                                }
                                _Result.Status = GenericStatus.Sucess;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetBooksListWithFilter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetBooksListWithFilter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = GenericStatus.Error;
            }
            return _Result;
        }

        public PagedList<Book> GetBooksListByFilter(int PageIndex, int PageSize, string SearchTxt, string SubSection, string Language, string BookType, bool HasAnimation, bool HasReadAloud, bool HasActivity)
        {
            PagedList<Book> _Result = new PagedList<Book>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBooksListByFilter", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        command.Parameters.Add("@SubSection", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SubSection) ? string.Empty : SubSection;
                        command.Parameters.Add("@Language", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(Language) ? string.Empty : Language;
                        command.Parameters.Add("@BookType", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(BookType) ? string.Empty : BookType;
                        if (HasAnimation)
                            command.Parameters.Add("@HasAnimation", SqlDbType.Bit).Value = HasAnimation;
                        if (HasActivity)
                            command.Parameters.Add("@HasActivity", SqlDbType.Bit).Value = HasActivity;
                        if (HasReadAloud)
                            command.Parameters.Add("@HasReadAloud", SqlDbType.Bit).Value = HasReadAloud;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables[0].Rows.Count != 0)
                            {

                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    Book _Book = new Book();
                                    _Book.BookId = int.Parse(_DataRow["BookId"].ToString());
                                    _Book.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString());
                                    _Book.KitabletID = _DataRow["KitabletID"].ToString();
                                    _Book.Thumbnail1 = _DataRow["Thumbnail1"].ToString();
                                    _Book.Thumbnail2 = _DataRow["Thumbnail2"].ToString();
                                    _Book.Thumbnail3 = _DataRow["Thumbnail3"].ToString();
                                    _Book.HasActivity = Convert.ToBoolean(_DataRow["HasActivity"]);
                                    _Book.HasReadAloud = Convert.ToBoolean(_DataRow["HasReadAloud"]);
                                    _Book.HasAnimation = Convert.ToBoolean(_DataRow["HasAnimation"]);
                                    _Book.ViewMode = _DataRow["ViewMode"].ToString();
                                    _Book.SubSection = _DataRow["SubSection"].ToString();
                                    _Book.Rating = _DataRow["Rating"] != DBNull.Value ? Convert.ToSingle(_DataRow["Rating"]) : 0;
                                    _Result.Items.Add(_Book);
                                }

                                _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                _Result.PageSize = PageSize;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetBooksListByFilter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _Result = null;
            }
            return _Result;
        }

        public List<ExportBook> GetBooksListForExport(string SubSection, string Language, string BookType, bool HasAnimation, bool HasReadAloud, bool HasActivity)
        {
            List<ExportBook> _Result = new List<ExportBook>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBooksListForExport", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SubSection", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SubSection) ? string.Empty : SubSection;
                        command.Parameters.Add("@Language", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(Language) ? string.Empty : Language;
                        command.Parameters.Add("@BookType", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(BookType) ? string.Empty : BookType;
                        if (HasAnimation)
                            command.Parameters.Add("@HasAnimation", SqlDbType.Bit).Value = HasAnimation;
                        if (HasActivity)
                            command.Parameters.Add("@HasActivity", SqlDbType.Bit).Value = HasActivity;
                        if (HasReadAloud)
                            command.Parameters.Add("@HasReadAloud", SqlDbType.Bit).Value = HasReadAloud;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    ExportBook _Book = new ExportBook();

                                    _Book = new ExportBook();
                                    _Book.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString());
                                    _Book.KitabletID = HttpUtility.HtmlDecode(_DataRow["KitabletID"].ToString());
                                    _Book.Author = HttpUtility.HtmlDecode(_DataRow["Author"].ToString());
                                    _Book.Illustrator = HttpUtility.HtmlDecode(_DataRow["Illustrator"].ToString());
                                    _Book.Publisher = HttpUtility.HtmlDecode(_DataRow["Publisher"].ToString());
                                    _Book.Translator = HttpUtility.HtmlDecode(_DataRow["Translator"].ToString());
                                    _Book.ShortDescription = HttpUtility.HtmlDecode(_DataRow["ShortDescription"].ToString());
                                    _Book.Language = _DataRow["Language"].ToString();
                                    _Book.SubSection = _DataRow["SubSection"].ToString();
                                    _Book.Genre = _DataRow["Genre"].ToString();
                                    _Book.Type = _DataRow["Type"].ToString();
                                    _Book.Enabled = bool.Parse(_DataRow["IsTrashed"].ToString()) ? "N" : "Y";

                                    _Result.Add(_Book);
                                }

                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetBooksListForExport", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _Result = null;
            }
            return _Result;
        }
        public StudentImportExportInput ImportStudents(List<StudentImportExport> Students, string SchoolUId)
        {
            StudentImportExportInput _Result = new StudentImportExportInput();
            var studentxml = new XElement("Students",
           from c in Students
           select new XElement("Student",
               new XElement("FirstName", HttpUtility.HtmlEncode(c.FirstName)),
               new XElement("LastName", HttpUtility.HtmlEncode(c.LastName)),
               new XElement("RollNo", c.RollNo),
               new XElement("Gender", c.Gender),
               new XElement("Grade", c.Grade),
               new XElement("SubSection", HttpUtility.HtmlEncode(c.SubSection)),
               new XElement("DateOfBirth", c.DateOfBirth),
               new XElement("SubscriptionStartDate", c.SubscriptionStartDate),
               new XElement("SubscriptionEndDate", c.SubscriptionEndDate),
               new XElement("ParentEmail", c.ParentEmail),
               new XElement("ParentMobileNumber", c.ParentMobileNumber)
               ));
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spImportStudents", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@XML", SqlDbType.Xml).Value = studentxml.ToString();
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        _SqlDataAdapter.Fill(_DataSet);

                        Status = (int)id.Value;
                        if (_DataSet.Tables.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                StudentImportExport _StudentImportExport = new StudentImportExport();
                                _StudentImportExport.SubSection = _DataRow["Section"].ToString();
                                _StudentImportExport.TotalSuccess = Convert.ToInt32(_DataRow["TotalSuccess"]);
                                _Result.Students.Add(_StudentImportExport);
                            }
                            _Result.Status = StudentsImportStatus.Sucess;
                            InsertLog(_currentUserId, "ImportStudents", "New students import from excel", string.Format("Students imported from excel in school {0}", SchoolUId), StudentsImportStatus.Sucess.ToString());
                        }
                        if (Status == 2)
                        {
                            InsertLog(_currentUserId, "ImportStudents", "New students import from excel", string.Format("Invalid SchoolUId {0}", SchoolUId), StudentsImportStatus.InvalidSchool.ToString());
                            _Result.Status = StudentsImportStatus.InvalidSchool;
                        }
                        else if (Status == -1)
                        {
                            InsertLog(_currentUserId, "ImportStudents", "New students import from excel", string.Format("Failed to import students from excel in school {0}", SchoolUId), StudentsImportStatus.Error.ToString());
                            _Result.Status = StudentsImportStatus.Error;
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "ImportStudents", string.Format("Error occured while importing studnets from excel in school {0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = StudentsImportStatus.Error;
            }
            return _Result;
        }
        public ChildImportExportInput ImportChildren(List<ChildImport> Students, string SchoolUId, string Password, string PasswordSalt)
        {
            ChildImportExportInput _Result = new ChildImportExportInput();
            var studentxml = new XElement("Students",
           from c in Students
           select new XElement("Student",
               new XElement("ChildFirstName", HttpUtility.HtmlEncode(c.ChildFirstName)),
               new XElement("ChildLastName", HttpUtility.HtmlEncode(c.ChildLastName)),               
               new XElement("Grade", c.Grade),
               new XElement("SubSection", HttpUtility.HtmlEncode(c.SubSection)),
               new XElement("SubscriptionStartDate", c.SubscriptionStartDate),
               new XElement("SubscriptionEndDate", c.SubscriptionEndDate),
               new XElement("ParentEmailID", c.ParentEmailID),
               new XElement("ParentFirstName", HttpUtility.HtmlEncode(c.ParentFirstName)),
               new XElement("ParentLastName", HttpUtility.HtmlEncode(c.ParentLastName))
               ));
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spImportChildren", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@XML", SqlDbType.Xml).Value = studentxml.ToString();
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = Password;
                        command.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar).Value = PasswordSalt;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        _SqlDataAdapter.Fill(_DataSet);

                        Status = (int)id.Value;
                        if (_DataSet.Tables.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                ChildImport _StudentImportExport = new ChildImport();
                                _StudentImportExport.SubSection = _DataRow["Section"].ToString();
                                _StudentImportExport.TotalSuccess = Convert.ToInt32(_DataRow["TotalSuccess"]);
                                _Result.Students.Add(_StudentImportExport);
                            }
                            _Result.Status = StudentsImportStatus.Sucess;
                            //InsertLog(_currentUserId, "ImportStudents", "New students import from excel", string.Format("Students imported from excel in school {0}", SchoolUId), StudentsImportStatus.Sucess.ToString());
                        }
                        if (Status == 2)
                        {
                            //InsertLog(_currentUserId, "ImportStudents", "New students import from excel", string.Format("Invalid SchoolUId {0}", SchoolUId), StudentsImportStatus.InvalidSchool.ToString());
                            _Result.Status = StudentsImportStatus.InvalidSchool;
                        }
                        else if (Status == -1)
                        {
                            //InsertLog(_currentUserId, "ImportStudents", "New students import from excel", string.Format("Failed to import students from excel in school {0}", SchoolUId), StudentsImportStatus.Error.ToString());
                            _Result.Status = StudentsImportStatus.Error;
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                //InsertLog(_currentUserId, "ImportStudents", string.Format("Error occured while importing studnets from excel in school {0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = StudentsImportStatus.Error;
            }
            return _Result;
        }
        public StudentImportExportInput UpdateMultipleStudents(List<StudentImportExport> Students, string SchoolUId)
        {
            StudentImportExportInput _Result = new StudentImportExportInput();
            var studentxml = new XElement("Students",
          from c in Students
          select new XElement("Student",
               new XElement("SNO", c.SNO),
              new XElement("FirstName", HttpUtility.HtmlEncode(c.FirstName)),
              new XElement("LastName", HttpUtility.HtmlEncode(c.LastName)),
              new XElement("RollNo", c.RollNo),
              new XElement("Gender", c.Gender),
              new XElement("Grade", c.Grade),
              new XElement("SubSection", HttpUtility.HtmlEncode(c.SubSection)),
              new XElement("SubscriptionStartDate", c.SubscriptionStartDate),
              new XElement("SubscriptionEndDate", c.SubscriptionEndDate),
              new XElement("IsRenew", c.IsRenew),
              new XElement("UniqueId", c.UniqueId)
              ));
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spBulkUpdateStudents", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@XML", SqlDbType.Xml).Value = studentxml.ToString();
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        _SqlDataAdapter.Fill(_DataSet);

                        Status = (int)id.Value;
                        if (_DataSet.Tables.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                StudentImportExport _StudentImportExport = new StudentImportExport();
                                _StudentImportExport.SubSection = _DataRow["Section"].ToString();
                                _StudentImportExport.TotalSuccess = Convert.ToInt32(_DataRow["TotalSuccess"]);
                                _Result.Students.Add(_StudentImportExport);
                            }
                            _Result.Status = StudentsImportStatus.Sucess;
                            InsertLog(_currentUserId, "UpdateMultipleStudents", "Bulk student update", string.Format("Bulk student update in school {0}", SchoolUId), StudentsImportStatus.Sucess.ToString());
                        }
                        if (Status == 2)
                        {
                            InsertLog(_currentUserId, "UpdateMultipleStudents", "Bulk student update", string.Format("Invalid SchoolUId {0}", SchoolUId), StudentsImportStatus.InvalidSchool.ToString());
                            _Result.Status = StudentsImportStatus.InvalidSchool;
                        }
                        else if (Status == -1)
                        {
                            InsertLog(_currentUserId, "UpdateMultipleStudents", "Bulk student update", string.Format("Failed to bulk student update in school {0}", SchoolUId), StudentsImportStatus.Error.ToString());
                            _Result.Status = StudentsImportStatus.Error;
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UpdateMultipleStudents", string.Format("Error occured in Bulk student update in school {0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = StudentsImportStatus.Error;
            }
            return _Result;
        }

        public UserStatus RegisterStudent(StudentRegistrationModel _Student)
        {
            int _id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spRegisterStudent", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = _Student.Username.ToLower();
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = _Student.Password;
                        command.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar).Value = _Student.PasswordSalt;
                        command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = _Student.UserId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        switch (_id)
                        {
                            case 1:
                                {
                                    InsertLog(_currentUserId, "RegisterStudent", "New student Registeration", string.Format("New user registered with username= {0}", _Student.Username), UserStatus.Sucess.ToString());
                                    return UserStatus.Sucess;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "RegisterStudent", "New student Registeration", string.Format("User already exists with username= {0}", _Student.Username), UserStatus.UserAlreadyRegistered.ToString());
                                    return UserStatus.UserAlreadyRegistered;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "RegisterStudent", "New student Registeration", string.Format("Failed to register new student with username= {0}", _Student.Username), UserStatus.Error.ToString());
                                    return UserStatus.Error;
                                }

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "RegisterStudent", string.Format("Error occured while registering new student with username= {0}", _Student.Username), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return UserStatus.Error;
            }
        }

        public bool IsUsernameUnique(string userName)
        {
            int _id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spIsUsernameUnique", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        if (_id == 0)
                            return false;
                        else
                            return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "IsUsernameUnique", string.Format("check user name uniqueness username= {0}", userName), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
        }

        public SchoolAdminDisableStatus DisableSchoolAdmin(int SchoolAdminId)
        {
            int _id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spDisableSchoolAdmins", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolAdminId", SqlDbType.Int).Value = SchoolAdminId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        switch (_id)
                        {
                            case 0:
                                {
                                    InsertLog(_currentUserId, "DisableSchoolAdmin", "Disable school admin", string.Format("Failed to disable school admin with Id {0}", SchoolAdminId), SchoolAdminDisableStatus.Error.ToString());
                                    return SchoolAdminDisableStatus.Error;
                                }
                            case 1:
                                {
                                    InsertLog(_currentUserId, "DisableSchoolAdmin", "Disable school admin", string.Format("School admin with Id {0} is disabled", SchoolAdminId), SchoolAdminDisableStatus.Success.ToString());
                                    return SchoolAdminDisableStatus.Success;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "DisableSchoolAdmin", "Disable school admin", "Can't disable last school admin", SchoolAdminDisableStatus.LastAdminDeletionNotallowed.ToString());
                                    return SchoolAdminDisableStatus.LastAdminDeletionNotallowed;
                                }
                            case 3:
                                {
                                    InsertLog(_currentUserId, "DisableSchoolAdmin", "Disable school admin", string.Format("No school admin with Id {0} to disable", SchoolAdminId), SchoolAdminDisableStatus.NotASchoolAdmin.ToString());
                                    return SchoolAdminDisableStatus.NotASchoolAdmin;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "DisableSchoolAdmin", "Disable school admin", string.Format("Failed to disable school admin with Id {0}", SchoolAdminId), SchoolAdminDisableStatus.Error.ToString());
                                    return SchoolAdminDisableStatus.Error;
                                }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "DisableSchoolAdmin", string.Format("Error occured while disabling school admin with Id {0}", SchoolAdminId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolAdminDisableStatus.Error;
            }
        }
        public ParentStudentDisableStatus DisableParentStudent(int UserId)
        {
            int _id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spDisableParentStudent", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        switch (_id)
                        {
                            case 1:
                                {
                                    InsertLog(_currentUserId, "DisableParentStudent", "Disable parent student", string.Format("User with Id {0} is disabled", UserId), ParentStudentDisableStatus.Success.ToString());
                                    return ParentStudentDisableStatus.Success;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "DisableParentStudent", "Disable parent student", string.Format("No user exists with Id {0}", UserId), ParentStudentDisableStatus.UserNotExist.ToString());
                                    return ParentStudentDisableStatus.UserNotExist;
                                }
                            case 3:
                                {
                                    InsertLog(_currentUserId, "DisableParentStudent", "Disable parent student", string.Format("Can't disable user with Id {0}", UserId), ParentStudentDisableStatus.ParentDeletionNotallowed.ToString());
                                    return ParentStudentDisableStatus.ParentDeletionNotallowed;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "DisableParentStudent", "Disable parent student", string.Format("Failed to disable user with Id {0}", UserId), ParentStudentDisableStatus.Error.ToString());
                                    return ParentStudentDisableStatus.Error;
                                }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "DisableParentStudent", string.Format("Error occured while disabling user with Id {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return ParentStudentDisableStatus.Error;
            }
        }

        public ElibAdminDisableStatus DisableELibAdmin(int UserId)
        {
            int _id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spDisableELibAdmins", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        switch (_id)
                        {
                            case 1:
                                {
                                    InsertLog(_currentUserId, "DisableELibAdmin", "Disable Elib Admin", string.Format("Elib admin {0} is disabled", UserId), UserStatus.Sucess.ToString());
                                    return ElibAdminDisableStatus.Success;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "DisableELibAdmin", "Disable Elib Admin", string.Format("No Elib admin with id {0} to disabled", UserId), ElibAdminDisableStatus.NotAElibAdmin.ToString());
                                    return ElibAdminDisableStatus.NotAElibAdmin;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "DisableELibAdmin", "Disable Elib Admin", string.Format("Failed to disable Elib admin {0}", UserId), UserStatus.Error.ToString());
                                    return ElibAdminDisableStatus.Error;
                                }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "DisableELibAdmin", string.Format("Error occured while disabling Elib admin {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return ElibAdminDisableStatus.Error;
            }
        }

        public BookDisableStatus DisableBook(int BookId)
        {
            int _id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spDisableBooks", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        switch (_id)
                        {
                            case 0:
                                {
                                    InsertLog(_currentUserId, "DisableBook", "disable book", string.Format("Failed to disable book {0}", BookId), UserStatus.Error.ToString());
                                    return BookDisableStatus.Error;
                                }
                            case 1:
                                {
                                    InsertLog(_currentUserId, "DisableBook", "disable book", string.Format("disable book {0}", BookId), BookDisableStatus.Success.ToString());
                                    return BookDisableStatus.Success;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "DisableBook", "disable book", string.Format("no book with id {0}", BookId), BookDisableStatus.NoBookExist.ToString());
                                    return BookDisableStatus.NoBookExist;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "DisableBook", "disable book", string.Format("Failed to disable book {0}", BookId), BookDisableStatus.Error.ToString());
                                    return BookDisableStatus.Error;
                                }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "DisableBook", string.Format("Error occured in disabling book {0}", BookId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return BookDisableStatus.Error;
            }
        }

        public SchoolDisableStatus DisableSchool(int SchoolId)
        {
            int _id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spDisableSchools", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _id = (int)id.Value;
                        switch (_id)
                        {
                            case 1:
                                {
                                    InsertLog(_currentUserId, "DisableSchool", "Disable school", string.Format("School {0} is disabled", SchoolId), SchoolDisableStatus.Success.ToString());
                                    return SchoolDisableStatus.Success;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "DisableSchool", "Disable school", string.Format("No School with id {0} to disable", SchoolId), SchoolDisableStatus.SchoolNotExist.ToString());
                                    return SchoolDisableStatus.SchoolNotExist;
                                }
                            case 3:
                                {
                                    InsertLog(_currentUserId, "DisableSchool", "Disable school", string.Format("Unable to disable school {0} ", SchoolId), SchoolDisableStatus.DeletionNotallowed.ToString());
                                    return SchoolDisableStatus.DeletionNotallowed;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "DisableSchool", "Disable school", string.Format("Failed to disable school {0} ", SchoolId), UserStatus.Error.ToString());
                                    return SchoolDisableStatus.Error;
                                }
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "DisableSchool", string.Format("Error occured while disabling school {0} ", SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolDisableStatus.Error;
            }
        }

        public BooksDetailsResult GetBookDetailsById(int BookId)
        {
            BooksDetailsResult _Result = new BooksDetailsResult();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBookDetails", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            int _id = (int)id.Value;
                            if (_DataSet.Tables.Count > 4 && _id != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    SubSection _Section = new SubSection();
                                    _Section.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Section.Name = _DataRow["Name"].ToString();
                                    _Section.IsSelected = Convert.ToBoolean(_DataRow["IsSelected"]);
                                    _Result.SubSection.Add(_Section);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    Language _Language = new Language();
                                    _Language.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Language.Name = _DataRow["Name"].ToString();
                                    _Language.IsSelected = Convert.ToBoolean(_DataRow["IsSelected"]);
                                    _Result.Language.Add(_Language);
                                }
                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    BookType _Type = new BookType();
                                    _Type.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Type.Name = _DataRow["Name"].ToString();
                                    _Type.IsSelected = Convert.ToBoolean(_DataRow["IsSelected"]);
                                    _Result.BookType.Add(_Type);
                                }
                                foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                {
                                    Genre _Genre = new Genre();
                                    _Genre.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Genre.Name = _DataRow["Name"].ToString();
                                    _Genre.IsSelected = Convert.ToBoolean(_DataRow["IsSelected"]);
                                    _Result.Genre.Add(_Genre);
                                }

                                if (_DataSet.Tables[4].Rows.Count > 0)
                                {
                                    DataRow _DataRow = _DataSet.Tables[4].Rows[0];
                                    Book _Book = new Book();
                                    _Book.BookId = int.Parse(_DataRow["BookId"].ToString());
                                    _Book.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString().Replace("\t", " "));
                                    _Book.Author = HttpUtility.HtmlDecode(_DataRow["Author"].ToString().Replace("\t", " "));
                                    _Book.Publisher = HttpUtility.HtmlDecode(_DataRow["Publisher"].ToString().Replace("\t", " "));
                                    _Book.Illustrator = HttpUtility.HtmlDecode(_DataRow["Illustrator"].ToString().Replace("\t", " "));
                                    _Book.Translator = HttpUtility.HtmlDecode(_DataRow["Translator"].ToString().Replace("\t", " "));
                                    _Book.Type = _DataRow["Type"].ToString();
                                    _Book.Genre = _DataRow["Genre"].ToString();
                                    _Book.Language = _DataRow["Language"].ToString();
                                    _Book.SubSection = _DataRow["SubSection"].ToString();
                                    _Book.Type = _DataRow["Type"].ToString();
                                    _Book.ViewMode = _DataRow["ViewMode"].ToString();
                                    _Book.HasAnimation = Convert.ToBoolean(_DataRow["HasAnimation"]);
                                    _Book.HasReadAloud = Convert.ToBoolean(_DataRow["HasReadAloud"]);
                                    _Book.HasActivity = Convert.ToBoolean(_DataRow["HasActivity"]);
                                    _Book.ShortDescription = HttpUtility.HtmlDecode(_DataRow["ShortDescription"].ToString().Replace("\t", " "));
                                    _Book.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                    _Book.Thumbnail1 = _DataRow["Thumbnail1"].ToString();
                                    _Book.Thumbnail2 = _DataRow["Thumbnail2"].ToString();
                                    _Book.Thumbnail3 = _DataRow["Thumbnail3"].ToString();
                                    _Book.Rating = _DataRow["Rating"] == DBNull.Value ? 0 : Convert.ToSingle(_DataRow["Rating"]);
                                    _Book.ViewMode = _DataRow["ViewMode"].ToString();
                                    _Result.Book = _Book;
                                }
                                _Result.Status = GenericStatus.Sucess;
                            }
                            else
                            {
                                _Result.Status = GenericStatus.Error;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetBookDetailsById", string.Format("Error occured in getting book {0}", BookId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetBookDetailsById", string.Format("Error occured in getting book {0}", BookId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = GenericStatus.Error;
            }
            return _Result;
        }

        public GenericStatus UpdateBooKMetaData(Book _book)
        {
            GenericStatus _Result;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateBooKMetaData", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = _book.BookId;
                        command.Parameters.Add("@Types", SqlDbType.NVarChar).Value = _book.Type;
                        command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_book.Title.Replace("\t", " "));
                        command.Parameters.Add("@ShortDescription", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_book.ShortDescription.Replace("\t", " "));
                        command.Parameters.Add("@Author", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_book.Author.Replace("\t", " "));
                        command.Parameters.Add("@Publisher", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_book.Publisher.Replace("\t", " "));
                        if (_book.Translator != null)
                            command.Parameters.Add("@Translator", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_book.Translator.Replace("\t", " "));
                        if (_book.Illustrator != null)
                            command.Parameters.Add("@Illustrator", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_book.Illustrator.Replace("\t", " "));
                        command.Parameters.Add("@HasReadAloud", SqlDbType.Bit).Value = _book.HasReadAloud;
                        command.Parameters.Add("@HasAnimation", SqlDbType.Bit).Value = _book.HasAnimation;
                        command.Parameters.Add("@HasActivity", SqlDbType.Bit).Value = _book.HasActivity;
                        command.Parameters.Add("@Genres", SqlDbType.NVarChar).Value = _book.Genre;
                        command.Parameters.Add("@Languages", SqlDbType.NVarChar).Value = _book.Language;
                        command.Parameters.Add("@SubSections", SqlDbType.NVarChar).Value = _book.SubSection;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);

                        try
                        {
                            int _id = 0;
                            if (con.State == ConnectionState.Open)
                                con.Close();
                            con.Open();
                            command.ExecuteNonQuery();
                            _id = (int)id.Value;
                            if (_id == 0)
                            {
                                InsertLog(_currentUserId, "UpdateBooKMetaData", "Update book meta data", string.Format("Failed to update book {0} meta data", _book.BookId), UserStatus.Error.ToString());
                                _Result = GenericStatus.Error;
                            }
                            else
                            {
                                InsertLog(_currentUserId, "UpdateBooKMetaData", "Update book meta data", string.Format("book {0} meta data update", _book.BookId), UserStatus.Sucess.ToString());
                                _Result = GenericStatus.Sucess;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "UpdateBooKMetaData", string.Format("Error occure in book {0} meta data update", _book.BookId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UpdateBooKMetaData", string.Format("Error occure in book {0} meta data update", _book.BookId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = GenericStatus.Error;
            }
            return _Result;
        }

        public UserBooksDetail GetBooksCatlog()
        {
            UserBooksDetail _Result = new UserBooksDetail();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBooksCatlog", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count >= 6)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    Genre _Genre = new Genre();
                                    _Genre.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Genre.Name = _DataRow["Name"].ToString();
                                    _Result.Genres.Add(_Genre);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    SubSection _Section = new SubSection();
                                    _Section.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Section.Name = _DataRow["Name"].ToString();
                                    _Section.ShortForm = _DataRow["ShortForm"].ToString();
                                    _Result.SubSections.Add(_Section);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    BookType _Type = new BookType();
                                    _Type.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Type.Name = _DataRow["Type"].ToString();
                                    _Result.BookTypes.Add(_Type);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                {
                                    Language _Language = new Language();
                                    _Language.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Language.Name = _DataRow["Name"].ToString();
                                    _Result.Languages.Add(_Language);
                                }

                                if (_DataSet.Tables[4].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[4].Rows)
                                    {
                                        UserBook _Book = new UserBook();

                                        _Book.Search.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString().Replace("\t", " "));
                                        _Book.Search.Author = HttpUtility.HtmlDecode(_DataRow["Author"].ToString().Replace("\t", " "));
                                        _Book.Search.Illustrator = HttpUtility.HtmlDecode(_DataRow["Illustrator"].ToString().Replace("\t", " "));
                                        _Book.Search.Translator = HttpUtility.HtmlDecode(_DataRow["Translator"].ToString().Replace("\t", " "));
                                        _Book.Search.Genre = _DataRow["Genres"].ToString();
                                        _Book.Search.Language = _DataRow["Languages"].ToString();
                                        _Book.Search.SubSection = HttpUtility.HtmlDecode(_DataRow["SubSections"].ToString());
                                        _Book.Search.Type = _DataRow["Types"].ToString();
                                        _Book.Search.HinglishTitle = _DataRow["HinglishTitle"].ToString();
                                        _Book.Search.Publisher = HttpUtility.HtmlDecode(_DataRow["Publisher"].ToString().Replace("\t", " "));
                                        _Book.Search.ISBN = _DataRow["ISBN"].ToString();
                                        _Book.Search.ShortDescription = HttpUtility.HtmlDecode(_DataRow["ShortDescription"].ToString().Replace("\t", " "));

                                        _Book.BookId = int.Parse(_DataRow["BookId"].ToString());
                                        _Book.FolderName = _DataRow["FolderName"].ToString();
                                        _Book.KitabletID = _DataRow["KitabletID"].ToString();
                                        _Book.YearOfPublication = Convert.ToInt32(_DataRow["YearOfPublication"]);
                                        _Book.ViewMode = _DataRow["ViewMode"].ToString();
                                        _Book.PageDisplay = _DataRow["PageDisplay"].ToString();
                                        _Book.NoOfPages = Convert.ToInt32(_DataRow["NoOfPages"]);
                                        _Book.HasActivity = Convert.ToBoolean(_DataRow["HasActivity"]);
                                        _Book.HasAnimation = Convert.ToBoolean(_DataRow["HasAnimation"]);
                                        _Book.HasReadAloud = Convert.ToBoolean(_DataRow["HasReadAloud"]);
                                        _Book.Thumbnail1 = _DataRow["Thumbnail1"].ToString();
                                        _Book.Thumbnail2 = _DataRow["Thumbnail2"].ToString();
                                        _Book.Thumbnail3 = _DataRow["Thumbnail3"].ToString();
                                        _Book.BackCover = _DataRow["BackCover"].ToString();
                                        _Book.Genres = _DataRow["GenreIds"].ToString();
                                        _Book.Languages = _DataRow["LanguageIds"].ToString();
                                        _Book.SubSections = _DataRow["SubSectionIds"].ToString();
                                        _Book.Types = _DataRow["TypeIds"].ToString();
                                        _Book.BookSize = _DataRow["BookSize"] != DBNull.Value ? Convert.ToInt32(_DataRow["BookSize"]) : 0;
                                        _Book.ActivityJson = _DataRow["ActivityJson"] != DBNull.Value ? Regex.Unescape(_DataRow["ActivityJson"].ToString()) : "";

                                        _Book.Rating.AverageRating = _DataRow["AverageRating"] != DBNull.Value ? Convert.ToDouble(_DataRow["AverageRating"]) : 0;
                                        _Book.Rating.TotalUserRatedThisBook = _DataRow["TotalUserRatedThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalUserRatedThisBook"]) : 0;
                                        _Book.Rating.FiveStarRating = _DataRow["TotalFiveRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalFiveRating"]) : 0;
                                        _Book.Rating.FourStarRating = _DataRow["TotalFourRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalFourRating"]) : 0;
                                        _Book.Rating.ThreeStarRating = _DataRow["TotalThreeRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalThreeRating"]) : 0;
                                        _Book.Rating.TwoStarRating = _DataRow["TotalTwoRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalTwoRating"]) : 0;
                                        _Book.Rating.OneStarRating = _DataRow["TotalOneRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalOneRating"]) : 0;

                                        _Book.TotalDownloads = _DataRow["TotalDownloads"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalDownloads"]) : 0;
                                        _Book.PopularityScore = _DataRow["PopularityScore"] != DBNull.Value ? Convert.ToInt32(_DataRow["PopularityScore"]) : 0;
                                        _Book.TotalReadingTimeOfThisBook = _DataRow["TotalReadingTimeOfThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalReadingTimeOfThisBook"]) : 0;
                                        _Book.TotalUsersReadThisBook = _DataRow["TotalUsersReadThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalUsersReadThisBook"]) : 0;
                                        _Book.TotalUsersCompletedReviewOfThisBook = _DataRow["TotalUsersCompletedReviewOfThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalUsersCompletedReviewOfThisBook"]) : 0;
                                        _Book.TotalUsersCompletedActivityOfThisBook = _DataRow["TotalUsersCompletedActivityOfThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalUsersCompletedActivityOfThisBook"]) : 0;
                                        _Book.TotalTimeForReviewOfThisBook = _DataRow["TotalTimeForReviewOfThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalTimeForReviewOfThisBook"]) : 0;
                                        _Book.TotalTimeForActivityOfThisBook = _DataRow["TotalTimeForActivityOfThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalTimeForActivityOfThisBook"]) : 0;

                                        _Book.ComingSoon = _DataRow["IsComingSoon"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsComingSoon"]) : false;
                                        _Book.Recommended = _DataRow["IsRecommended"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsRecommended"]) : false;
                                        _Book.IsPagerAllowed = _DataRow["IsPagerAllowed"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsPagerAllowed"]) : false;
                                        _Result.Books.Add(_Book);
                                    }
                                    _Result.BooksCount = _Result.Books.Count;
                                }

                                if (_DataSet.Tables[5].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[5].Rows)
                                    {
                                        Tag _Tag = new Tag();
                                        _Tag.Id = int.Parse(_DataRow["Id"].ToString());
                                        _Tag.Text = _DataRow["Text"].ToString();
                                        _Tag.Type = _DataRow["Type"].ToString();
                                        _Result.Tags.Add(_Tag);
                                    }
                                }
                                if (_DataSet.Tables[6].Rows.Count != 0)
                                {
                                    _Result.ReviewJson = Regex.Unescape(_DataSet.Tables[6].Rows[0]["Json"].ToString());
                                }

                                _Result.Status = GenericStatus.Sucess;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetBooksCatlog", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _Result.Status = GenericStatus.Error;
            }
            return _Result;
        }

        public bool ReleaseUserBooks(int UserId, string BookIds, int DeviceId)
        {
            bool _Result = false;
            int Status = 0;
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spReleaseUserBooks", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@BookIds", SqlDbType.NVarChar).Value = BookIds;
                        command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = DeviceId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            _Result = true;
                            InsertLog(_currentUserId, "ReleaseUserBooks", "Release User Books", string.Format("Release Books {0} from DeviceId {1} for User {2}", BookIds, DeviceId, UserId), UserStatus.Sucess.ToString());
                        }
                        else
                        {
                            _Result = false;
                            InsertLog(_currentUserId, "ReleaseUserBooks", "Release User Books", string.Format("Failed to release Books {0} from DeviceId {1} for User {2}", BookIds, DeviceId, UserId), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _Result = false;
                InsertLog(_currentUserId, "ReleaseUserBooks", string.Format("Error occured while releasing Books {0} from DeviceId {1} for User {2}", BookIds, DeviceId, UserId), ex.Message.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public bool ReleaseUserDevices(int UserId, string DeviceIds)
        {
            bool _Result = false;
            int Status = 0;
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spReleaseUserDevices", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@DeviceIds", SqlDbType.NVarChar).Value = DeviceIds;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            _Result = true;
                            InsertLog(_currentUserId, "ReleaseUserDevices", "Release User Device", string.Format("Release Device {0} from User {1}", DeviceIds, UserId), UserStatus.Sucess.ToString());
                        }
                        else
                        {
                            _Result = false;
                            InsertLog(_currentUserId, "ReleaseUserDevices", "Release User Device", string.Format("Failed to release Device {0} from User {1}", DeviceIds, UserId), UserStatus.Error.ToString());
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                _Result = false;
                InsertLog(_currentUserId, "ReleaseUserDevices", string.Format("Failed to release Device {0} from User {1}", DeviceIds, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }
        public GenericStatus UserDownloadBook(int UserId, int BookId, int DeviceId)
        {
            int Status = 0;
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUserDownloadBook", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;
                        command.Parameters.Add("@DeviceId", SqlDbType.Int).Value = DeviceId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(_currentUserId, "UserDownloadBook", "Book Download on device", string.Format("BookId = {0} is downloded on Device = {1} for user {2}", BookId, DeviceId, UserId), UserStatus.Sucess.ToString());
                            return GenericStatus.Sucess;
                        }
                        else
                        {
                            InsertLog(_currentUserId, "UserDownloadBook", "Book Download on device", string.Format("Failed to download BookId = {0} is downloded on Device = {1} for user {2}", BookId, DeviceId, UserId), UserStatus.Error.ToString());
                            return GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UserDownloadBook", string.Format("Error occured while downloading BookId = {0} is downloded on Device = {1} for user {2}", BookId, DeviceId, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return GenericStatus.Error;
            }
        }
        public ReadBookStatus ReadBook(int UserId, string DeviceDetails, int MaxCopies, int BookId)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spReadBook", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@DeviceDetails", SqlDbType.NVarChar).Value = DeviceDetails;
                        command.Parameters.Add("@MaxCopies", SqlDbType.Int).Value = MaxCopies;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        switch (Status)
                        {
                            case 0:
                                {
                                    InsertLog(_currentUserId, "ReadBook", "", "", ReadBookStatus.Error.ToString());
                                    return ReadBookStatus.Error;
                                }
                            case 1:
                                {
                                    InsertLog(_currentUserId, "ReadBook", "", "", ReadBookStatus.SubscriptionExpires.ToString());
                                    return ReadBookStatus.SubscriptionExpires;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "ReadBook", "", "", ReadBookStatus.AlreadyDownloaded.ToString());
                                    return ReadBookStatus.AlreadyDownloaded;
                                }
                            case 3:
                                {
                                    InsertLog(_currentUserId, "ReadBook", "", "", ReadBookStatus.NoMoreCopiesAllowed.ToString());
                                    return ReadBookStatus.NoMoreCopiesAllowed;
                                }
                            case 4:
                                {
                                    InsertLog(_currentUserId, "ReadBook", "", "", ReadBookStatus.Download.ToString());
                                    return ReadBookStatus.Download;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "ReadBook", "", "", ReadBookStatus.Error.ToString());
                                    return ReadBookStatus.Error;
                                }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "ReadBook", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return ReadBookStatus.Error;
            }
        }

        public GenericStatus ReadLater(int userId, int bookId)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spReadLater", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                        command.Parameters.Add("@bookId", SqlDbType.Int).Value = bookId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        switch (Status)
                        {
                            case 0:
                                {
                                    InsertLog(_currentUserId, "ReadLater", string.Format("Failed to set read later book {0} for user {1}", bookId, userId), string.Format("Failed to set read later book {0} for user {1}", bookId, userId), GenericStatus.Error.ToString());
                                    return GenericStatus.Other;
                                }
                            case 1:
                                {
                                    InsertLog(_currentUserId, "ReadLater", string.Format("Set read later book {0} for user {1}", bookId, userId), string.Format("Set read later book {0} for user {1}", bookId, userId), GenericStatus.Sucess.ToString());
                                    return GenericStatus.Sucess;
                                }
                            case 2:
                                {
                                    InsertLog(_currentUserId, "ReadLater", string.Format("No active book exits to set read later book {0} for user {1}", bookId, userId), string.Format("No active book exits to set read later book {0} for user {1}", bookId, userId), GenericStatus.Other.ToString());
                                    return GenericStatus.Other;
                                }
                            default:
                                {
                                    InsertLog(_currentUserId, "ReadLater", string.Format("Failed to set read later book {0} for user {1}", bookId, userId), string.Format("Failed to set read later book {0} for user {1}", bookId, userId), GenericStatus.Error.ToString());
                                    return GenericStatus.Error;
                                }
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "ReadLater", string.Format("Error occured while setting read later book {0} for user {1}", bookId, userId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return GenericStatus.Error;
            }
        }

        public SearchResult Search(string Role, int PageIndex, int PageSize, string SearchTxt, int UserId)
        {
            SearchResult _Result = new SearchResult();
            string[] Books = new string[] { "superadmin", "elibadmin" };
            string[] Students = new string[] { "superadmin", "elibadmin", "schooladmin" };
            string[] SchoolsAndAdmins = new string[] { "superadmin", "elibadmin" };
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSearch", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = SearchTxt;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count > 3)
                            {
                                if (SchoolsAndAdmins.Contains(Role))
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        School _School = new School();
                                        _School.SchoolName = HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Name"]));
                                        _School.SchoolUId = _DataRow["SchoolUId"].ToString();
                                        _School.StudentCount = _DataRow["StudentCount"] != DBNull.Value ? Convert.ToInt32(_DataRow["StudentCount"]) : 0;
                                        _School.SchoolAdminCount = _DataRow["SchoolAdminCount"] != DBNull.Value ? Convert.ToInt32(_DataRow["SchoolAdminCount"]) : 0;
                                        _School.IsEmailVerified = _DataRow["IsEmailVerified"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsEmailVerified"]) : false;
                                        _School.IsActive = Convert.ToBoolean(_DataRow["IsActive"]);
                                        _School.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                        _Result.School.Items.Add(_School);
                                    }
                                    _Result.School.PageIndex = PageIndex;
                                    _Result.School.PageSize = PageSize;
                                    _Result.School.TotalItems = _DataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["TotalRows"]) : 0;


                                    foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                    {
                                        StudentModel _User = new StudentModel();
                                        _User.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _User.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _User.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _User.Email = _DataRow["Email"].ToString();
                                        _User.MobileNumber = _DataRow["MobileNo"].ToString();
                                        _User.Status = Convert.ToBoolean(_DataRow["Status"]);
                                        _User.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                        _User.SchoolUId = _DataRow["SchoolUId"].ToString();
                                        _Result.SchoolAdmin.Items.Add(_User);
                                    }
                                    _Result.SchoolAdmin.PageIndex = PageIndex;
                                    _Result.SchoolAdmin.PageSize = PageSize;
                                    _Result.SchoolAdmin.TotalItems = _DataSet.Tables[1].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[1].Rows[0]["TotalRows"]) : 0; ;
                                }
                                if (Students.Contains(Role))
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                    {
                                        StudentModel _Student = new StudentModel();
                                        _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.Email = _DataRow["Email"].ToString();
                                        _Student.SubscriptionStartDate = Convert.ToDateTime(_DataRow["SubscriptionStartDate"]);
                                        _Student.SubscriptionEndDate = Convert.ToDateTime(_DataRow["SubscriptionEndDate"]);
                                        _Student.Status = Convert.ToBoolean(_DataRow["Status"]);
                                        _Student.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                        _Student.SchoolUId = _DataRow["SchoolUId"].ToString();
                                        _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                        _Result.Student.Items.Add(_Student);
                                    }
                                    _Result.Student.PageIndex = PageIndex;
                                    _Result.Student.PageSize = PageSize;
                                    _Result.Student.TotalItems = _DataSet.Tables[2].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[2].Rows[0]["TotalRows"]) : 0; ;
                                }

                                if (Books.Contains(Role))
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                    {
                                        Book _Book = new Book();
                                        _Book.BookId = int.Parse(_DataRow["BookId"].ToString());
                                        _Book.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString().Replace("\t", " "));
                                        _Book.KitabletID = _DataRow["KitabletID"].ToString();
                                        _Book.Thumbnail1 = _DataRow["Thumbnail1"].ToString();
                                        _Book.Thumbnail2 = _DataRow["Thumbnail2"].ToString();
                                        _Book.Thumbnail3 = _DataRow["Thumbnail3"].ToString();
                                        _Book.HasActivity = Convert.ToBoolean(_DataRow["HasActivity"]);
                                        _Book.HasAnimation = Convert.ToBoolean(_DataRow["HasAnimation"]);
                                        _Book.HasReadAloud = Convert.ToBoolean(_DataRow["HasReadAloud"]);
                                        _Book.Rating = _DataRow["Rating"] != DBNull.Value ? Convert.ToSingle(_DataRow["Rating"]) : 0;
                                        _Book.ViewMode = _DataRow["ViewMode"].ToString();
                                        _Book.SubSection = _DataRow["SubSection"].ToString();
                                        _Result.Book.Items.Add(_Book);
                                    }

                                    _Result.Book.TotalItems = _DataSet.Tables[3].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[3].Rows[0]["TotalRows"]) : 0; ;
                                    _Result.Book.PageSize = PageSize;
                                    _Result.Book.PageIndex = PageIndex;
                                }

                                _Result.Status = GenericStatus.Sucess;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "Search", string.Format("Error occured in search searchstring={0} with user {1}", SearchTxt, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "Search", string.Format("Error occured in search searchstring={0} with user {1}", SearchTxt, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = GenericStatus.Error;
            }
            return _Result;
        }

        public PagedList<StudentModel> SearchStudent(int PageIndex, int PageSize, string SearchTxt, int UserId)
        {
            PagedList<StudentModel> _Result = new PagedList<StudentModel>();

            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSearchStudentsOfSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = SearchTxt;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    StudentModel _Student = new StudentModel();
                                    _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Student.Email = _DataRow["Email"].ToString();
                                    _Student.SubscriptionStartDate = Convert.ToDateTime(_DataRow["SubscriptionStartDate"]);
                                    _Student.SubscriptionEndDate = Convert.ToDateTime(_DataRow["SubscriptionEndDate"]);
                                    _Student.Status = Convert.ToBoolean(_DataRow["Status"]);
                                    _Student.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                    _Student.SchoolUId = _DataRow["SchoolUId"].ToString();
                                    _Student.LastLoginDate = _DataRow["LastLoginDate"]!= DBNull.Value ? Convert.ToDateTime(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Items.Add(_Student);
                                }
                                _Result.PageIndex = PageIndex;
                                _Result.PageSize = PageSize;
                                _Result.TotalItems = _DataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["TotalRows"]) : 0; ;

                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "SearchStudent", string.Format("Error occured in SearchStudent searchstring={0} with user {1}", SearchTxt, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SearchStudent", string.Format("Error occured in SearchStudent searchstring={0} with user {1}", SearchTxt, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _Result;
        }

        public PagedList<StudentModel> SearchSchoolAdmin(int PageIndex, int PageSize, string SearchTxt)
        {
            PagedList<StudentModel> _Result = new PagedList<StudentModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSearchSchoolAdmins", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = SearchTxt;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    StudentModel _User = new StudentModel();
                                    _User.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _User.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _User.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _User.Email = _DataRow["Email"].ToString();
                                    _User.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _User.Status = Convert.ToBoolean(_DataRow["Status"]);
                                    _User.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                    _User.SchoolUId = _DataRow["SchoolUId"].ToString();
                                    _User.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Items.Add(_User);
                                }
                                _Result.PageIndex = PageIndex;
                                _Result.PageSize = PageSize;
                                _Result.TotalItems = _DataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["TotalRows"]) : 0; ;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "SearchSchoolAdmin", string.Format("Error occured in SearchSchoolAdmin searchstring={0}", SearchTxt), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SearchSchoolAdmin", string.Format("Error occured in SearchSchoolAdmin searchstring={0}", SearchTxt), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _Result;
        }

        public PagedList<School> SearchSchoolList(int PageIndex, int PageSize, string SearchTxt)
        {
            PagedList<School> _Result = new PagedList<School>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSearchSchoolList", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    School _School = new School();
                                    _School.SchoolName = HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Name"]));
                                    _School.SchoolUId = _DataRow["SchoolUId"].ToString();
                                    _School.StudentCount = _DataRow["StudentCount"] != DBNull.Value ? Convert.ToInt32(_DataRow["StudentCount"]) : 0;
                                    _School.SchoolAdminCount = _DataRow["SchoolAdminCount"] != DBNull.Value ? Convert.ToInt32(_DataRow["SchoolAdminCount"]) : 0;
                                    _School.IsEmailVerified = _DataRow["IsEmailVerified"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsEmailVerified"]) : false;
                                    _School.IsActive = Convert.ToBoolean(_DataRow["IsActive"]);
                                    _School.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                    _Result.Items.Add(_School);
                                }
                                _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                _Result.PageSize = PageSize;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "SearchSchoolList", string.Format("Error occured in SearchSchoolList searchstring={0}", SearchTxt), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SearchSchoolList", string.Format("Error occured in SearchSchoolList searchstring={0}", SearchTxt), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public PagedList<Book> SearchBooksList(int PageIndex, int PageSize, string SearchTxt)
        {
            PagedList<Book> _Result = new PagedList<Book>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSearchBooksList", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables[0].Rows.Count != 0)
                            {

                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    Book _Book = new Book();
                                    _Book.BookId = int.Parse(_DataRow["BookId"].ToString());
                                    _Book.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString());
                                    _Book.KitabletID = _DataRow["KitabletID"].ToString();
                                    _Book.Thumbnail1 = _DataRow["Thumbnail1"].ToString();
                                    _Book.Thumbnail2 = _DataRow["Thumbnail2"].ToString();
                                    _Book.Thumbnail3 = _DataRow["Thumbnail3"].ToString();
                                    _Book.HasActivity = Convert.ToBoolean(_DataRow["HasActivity"]);
                                    _Book.HasReadAloud = Convert.ToBoolean(_DataRow["HasReadAloud"]);
                                    _Book.HasAnimation = Convert.ToBoolean(_DataRow["HasAnimation"]);
                                    _Book.Rating = _DataRow["Rating"] != DBNull.Value ? Convert.ToSingle(_DataRow["Rating"]) : 0;
                                    _Book.ViewMode = _DataRow["ViewMode"].ToString();
                                    _Book.SubSection = _DataRow["SubSection"].ToString();
                                    _Result.Items.Add(_Book);
                                }

                                _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                _Result.PageSize = PageSize;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "SearchBooksList", string.Format("Error occured in SearchBooksList searchstring={0}", SearchTxt), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SearchBooksList", string.Format("Error occured in SearchBooksList searchstring={0}", SearchTxt), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public string GetBlobPathOfBook(int BookId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBlobPathOfBook", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count != 0)
                            {
                                if (_DataSet.Tables[0].Rows.Count > 0)
                                {
                                    return _DataSet.Tables[0].Rows[0]["BlobFilePath"].ToString();
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                        catch (SqlException ex)
                        {
                            return null;
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return null;
        }

        public ParentDashboardResult GetParentDashbord(int UserId)
        {
            ParentDashboardResult _Result = new ParentDashboardResult();
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetParentDashboard", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    ParentDashboard _ParentDashboard = new ParentDashboard();
                                    _ParentDashboard.UserId = Convert.ToInt32(_DataRow["UserId"]);
                                    _ParentDashboard.FirstName = HttpUtility.HtmlDecode(Convert.ToString(_DataRow["FirstName"]));
                                    _ParentDashboard.LastName = HttpUtility.HtmlDecode(Convert.ToString(_DataRow["LastName"]));
                                    _ParentDashboard.SubscriptionStartDate = Convert.ToDateTime(_DataRow["SubscriptionStartDate"]);
                                    _ParentDashboard.SubscriptionEndDate = Convert.ToDateTime(_DataRow["SubscriptionEndDate"]);
                                    _ParentDashboard.BooksRead = _DataRow["BooksRead"] != DBNull.Value ? Convert.ToInt32(_DataRow["BooksRead"]) : 0;
                                    _ParentDashboard.BooksRated = _DataRow["BooksRated"] != DBNull.Value ? Convert.ToInt32(_DataRow["BooksRated"]) : 0;
                                    _ParentDashboard.HourSpent = _DataRow["HourSpent"] != DBNull.Value ? Convert.ToInt32(_DataRow["HourSpent"]) : 0;
                                    _ParentDashboard.ActivitiesCompleted = _DataRow["ActivitiesCompleted"] != DBNull.Value ? Convert.ToInt32(_DataRow["ActivitiesCompleted"]) : 0;
                                    _ParentDashboard.SchoolName = HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Name"]));
                                    _ParentDashboard.Status = Convert.ToBoolean(_DataRow["Status"]);
                                    _ParentDashboard.SchoolUId = Convert.ToString(_DataRow["SchoolUId"]);
                                    _ParentDashboard.SchoolIsTrashed = Convert.ToBoolean(_DataRow["SchoolIsTrashed"]);
                                    _ParentDashboard.IsActive = Convert.ToBoolean(_DataRow["IsActive"]);
                                    _ParentDashboard.Status = Convert.ToBoolean(_DataRow["Status"]);
                                    _ParentDashboard.IsTrashed = Convert.ToBoolean(_DataRow["IsTrashed"]);
                                    _ParentDashboard.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? Convert.ToDateTime(_DataRow["RegistrationDate"]) : (DateTime?)null;
                                    _Result.Students.Add(_ParentDashboard);
                                }
                                _Result.Status = GenericStatus.Sucess;
                            }
                            else
                            {
                                _Result.Status = GenericStatus.Error;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetParentDashbord", string.Format("Error occured in getting parent {0} dashboard", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetParentDashbord", string.Format("Error occured in getting parent {0} dashboard", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                //throw ex;
            }
            return _Result;
        }

        public ElibSuperAdminDashboardResult GetElibSuperAdminDashboard()
        {
            ElibSuperAdminDashboardResult _Result = new ElibSuperAdminDashboardResult();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetElibSuperAdminDashboard", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 2)
                            {
                                if (_DataSet.Tables[0].Rows.Count > 0)
                                {
                                    _Result.BookCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["BookCount"]);
                                    _Result.CitieCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["CitieCount"]);
                                    _Result.StudentCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["StudentCount"]);
                                    _Result.SchoolCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["SchoolCount"]);
                                    _Result.LibrarianCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["LibrarianCount"]);
                                    _Result.HindiBookCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["HindiBookCount"]);
                                    _Result.EnglishBookCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["EnglishBookCount"]);
                                    _Result.BilingualBookCount = Convert.ToInt32(_DataSet.Tables[0].Rows[0]["BilingualBookCount"]);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    SubSection _SubSection = new SubSection();
                                    _SubSection.Id = Convert.ToInt32(_DataRow["Id"]);
                                    _SubSection.Name = Convert.ToString(_DataRow["Name"]);
                                    _SubSection.BooksCount = Convert.ToInt32(_DataRow["BooksCount"]);
                                    _Result.SubSections.Add(_SubSection);
                                }
                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    School _School = new School();
                                    _School.SchoolId = Convert.ToInt32(_DataRow["SchoolId"]);
                                    _School.SchoolUId = Convert.ToString(_DataRow["SchoolUId"]);
                                    _School.SchoolName = HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Name"]));
                                    _School.StudentCount = _DataRow["StudentCount"] != DBNull.Value ? Convert.ToInt32(_DataRow["StudentCount"]) : 0;
                                    _School.SchoolAdminCount = _DataRow["AdminCount"] != DBNull.Value ? Convert.ToInt32(_DataRow["AdminCount"]) : 0;
                                    _Result.Schools.Add(_School);
                                }
                                _Result.Status = GenericStatus.Sucess;
                            }
                            else
                            {
                                _Result.Status = GenericStatus.Error;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetElibSuperAdminDashboard", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetElibSuperAdminDashboard", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                //throw ex;
            }
            return _Result;
        }

        public UsernameRecoveryStatus UserNameRecovery(string Email)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUserNameRecovery", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;
                        if (_Status == 0)
                        {
                            InsertLog(_currentUserId, "UserNameRecovery", "User name recovery", string.Format("Failed to recover user name for email {0}", Email), UsernameRecoveryStatus.Error.ToString());
                            return UsernameRecoveryStatus.Error;
                        }
                        else if (_Status == 2)
                        {
                            InsertLog(_currentUserId, "UserNameRecovery", "User name recovery", string.Format("User with email {0} is not active", Email), UsernameRecoveryStatus.AccountIsNotActive.ToString());
                            return UsernameRecoveryStatus.AccountIsNotActive;
                        }
                        else if (_Status == 3)
                        {
                            InsertLog(_currentUserId, "UserNameRecovery", "User name recovery", string.Format("User with email {0} is disabled", Email), UsernameRecoveryStatus.AccountIsDisabled.ToString());
                            return UsernameRecoveryStatus.AccountIsDisabled;
                        }
                        else if (_Status == 4)
                        {
                            InsertLog(_currentUserId, "UserNameRecovery", "User name recovery", string.Format("User with email {0} is not exists", Email), UsernameRecoveryStatus.NoUserexist.ToString());
                            return UsernameRecoveryStatus.NoUserexist;
                        }
                        else
                        {
                            InsertLog(_currentUserId, "UserNameRecovery", "User name recovery", string.Format("User name recovered for email {0}", Email), UsernameRecoveryStatus.Success.ToString());
                            return UsernameRecoveryStatus.Success;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UserNameRecovery", string.Format("Error occured while recovering user name for email {0}", Email), ex.InnerException.ToString(), UsernameRecoveryStatus.Error.ToString());
                return UsernameRecoveryStatus.Error;
            }
        }

        public SchoolRegistrationEmailStatus SchoolRegistrationEmail(int SchoolId)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSchoolRegistrationEmail", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;
                        if (_Status == 0)
                        {
                            InsertLog(_currentUserId, "SchoolRegistrationEmail", string.Format("Failed to send verifiocation email for school {0}", SchoolId), string.Format("Failed to send verifiocation email for school {0}", SchoolId), UserStatus.Error.ToString());
                            return SchoolRegistrationEmailStatus.Error;
                        }
                        else if (_Status == 1)
                        {
                            InsertLog(_currentUserId, "SchoolRegistrationEmail", string.Format("Verifiocation email send for school {0}", SchoolId), string.Format("Send verifiocation email for school {0}", SchoolId), UserStatus.Sucess.ToString());
                            return SchoolRegistrationEmailStatus.Success;
                        }
                        else
                        {
                            InsertLog(_currentUserId, "SchoolRegistrationEmail", "Verification email for school", string.Format("No school exists with id {0}", SchoolId), SchoolRegistrationEmailStatus.NoSchoolExist.ToString());
                            return SchoolRegistrationEmailStatus.NoSchoolExist;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SchoolRegistrationEmail", string.Format("Error occured while sending verifiocation email for school {0}", SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return SchoolRegistrationEmailStatus.Error;
            }
        }

        public StudentProfileResult GetStudentProfile(int UserId, string SchoolUId)
        {
            StudentProfileResult _Result = new StudentProfileResult();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudentProfile", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@SchoolUId", SqlDbType.NVarChar).Value = SchoolUId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            int _Status = (int)Status.Value;
                            if (_DataSet.Tables.Count != 0 && _DataSet.Tables[0].Rows.Count > 0 && _Status == 1)
                            {
                                _Result.UserId = int.Parse(_DataSet.Tables[0].Rows[0]["UserId"].ToString());
                                _Result.FirstName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["FirstName"].ToString());
                                _Result.LastName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["LastName"].ToString());
                                _Result.Grade = _DataSet.Tables[0].Rows[0]["Grade"].ToString();
                                _Result.Status = Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["Status"]);
                                _Result.SubscriptionStartDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["SubscriptionStartDate"].ToString());
                                _Result.SubscriptionEndDate = DateTime.Parse(_DataSet.Tables[0].Rows[0]["SubscriptionEndDate"].ToString());
                                _Result.Username = _DataSet.Tables[0].Rows[0]["UserName"].ToString();
                                _Result.HomeDevices = _DataSet.Tables[0].Rows[0]["HomeDevices"].ToString();
                                _Result.SchoolName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["Name"].ToString());
                                _Result.SchoolUId = _DataSet.Tables[0].Rows[0]["SchoolUId"].ToString();
                                _Result.SubSection = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["SubSection"].ToString());
                                _Result.DateOfBirth = _DataSet.Tables[0].Rows[0]["DateOfBirth"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(_DataSet.Tables[0].Rows[0]["DateOfBirth"].ToString());
                                _Result.Gender = _DataSet.Tables[0].Rows[0]["Gender"].ToString();
                                _Result.Grades = _DataSet.Tables[0].Rows[0]["Grades"].ToString();
                                _Result.CreationDate = Convert.ToDateTime(_DataSet.Tables[0].Rows[0]["CreationDate"].ToString());
                                _Result.RegistrationDate = _DataSet.Tables[0].Rows[0]["RegistrationDate"] != DBNull.Value ? Convert.ToDateTime(_DataSet.Tables[0].Rows[0]["RegistrationDate"].ToString()) : (DateTime?)null;
                                _Result.LastLoginDate = _DataSet.Tables[0].Rows[0]["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(_DataSet.Tables[0].Rows[0]["LastLoginDate"].ToString()) : (DateTime?)null;
                                _Result.APIStatus = UserStatus.Sucess;
                            }
                            else
                            {
                                _Result.APIStatus = UserStatus.UserAccountNotExist;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetStudentProfile", string.Format("Error occured in getting student {0} profile", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.APIStatus = UserStatus.Error;
                        }
                    }
                }
                return _Result;
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetStudentProfile", string.Format("Error occured in getting student {0} profile", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
        }

        public DataSet GetParentProfile(int UserId, string SchoolUId)
        {
            DataSet ds = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetParentProfile", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@SchoolUId", SqlDbType.NVarChar).Value = SchoolUId;
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetParentProfile", string.Format("Error occured in getting parent {0} profile", UserId), ex.Message.ToString(), UserStatus.Error.ToString());
                throw ex;
            }
        }
        public List<StudentImportExport> ValidateImportStudents(List<StudentImportExport> Students)
        {
            var studentxml = new XElement("Students",
            from c in Students
            select new XElement("Student",
                new XElement("SNO", c.SNO),
                new XElement("FirstName", HttpUtility.HtmlEncode(c.FirstName)),
                new XElement("LastName", HttpUtility.HtmlEncode(c.LastName)),
                new XElement("RollNo", c.RollNo),
                new XElement("Gender", c.Gender),
                new XElement("Grade", c.Grade),
                new XElement("SubSection", c.SubSection),
                new XElement("DateOfBirth", c.DateOfBirth),
                new XElement("SubscriptionStartDate", c.SubscriptionStartDate),
                new XElement("SubscriptionEndDate", c.SubscriptionEndDate),
                new XElement("ParentEmail", c.ParentEmail),
                new XElement("ParentMobileNumber", c.ParentMobileNumber)
                ));
            List<StudentImportExport> _Result = new List<StudentImportExport>();
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spValiadteImportStudents", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Students", SqlDbType.Xml).Value = studentxml.ToString(); ;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        _SqlDataAdapter.Fill(_DataSet);

                        Status = (int)id.Value;
                        if (_DataSet.Tables.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                StudentImportExport _StudentImportExport = new StudentImportExport();
                                _StudentImportExport.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                _StudentImportExport.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                _StudentImportExport.Grade = _DataRow["Grade"].ToString();
                                _StudentImportExport.SubSection = HttpUtility.HtmlDecode(_DataRow["SubSection"].ToString());
                                _StudentImportExport.RollNo = _DataRow["RollNo"].ToString();
                                _StudentImportExport.Gender = _DataRow["Gender"].ToString();
                                _StudentImportExport.ParentEmail = _DataRow["ParentEmail"].ToString();
                                _StudentImportExport.ParentMobileNumber = _DataRow["ParentMobileNumber"].ToString();
                                _StudentImportExport.DateOfBirth = string.IsNullOrEmpty(_DataRow["DateOfBirth"].ToString()) ? (DateTime?)null : Convert.ToDateTime(_DataRow["DateOfBirth"]);
                                _StudentImportExport.SubscriptionStartDate = Convert.ToDateTime(_DataRow["SubscriptionStartDate"]);
                                _StudentImportExport.SubscriptionEndDate = Convert.ToDateTime(_DataRow["SubscriptionEndDate"]);
                                _StudentImportExport.Status = Convert.ToBoolean(_DataRow["Status"]);
                                _StudentImportExport.RowNumber = Convert.ToInt32(_DataRow["RowNumber"]);
                                _StudentImportExport.SNO = Convert.ToInt32(_DataRow["SNO"]);
                                _StudentImportExport.GradeStatus = Convert.ToBoolean(_DataRow["GradeStatus"]);
                                _Result.Add(_StudentImportExport);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "ValidateImportStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _Result;
        }

        public List<ChildImport> ValidateImportChildren(List<ChildImport> Students)
        {
            var studentxml = new XElement("Students",
            from c in Students
            select new XElement("Student",
                new XElement("SNO", c.SNO),
                new XElement("FirstName", HttpUtility.HtmlEncode(c.ChildFirstName)),
                new XElement("LastName", HttpUtility.HtmlEncode(c.ChildLastName)),
                new XElement("Grade", c.Grade),
                new XElement("SubSection", c.SubSection),
                new XElement("SubscriptionStartDate", c.SubscriptionStartDate),
                new XElement("SubscriptionEndDate", c.SubscriptionEndDate),
                new XElement("ParentFirstName", HttpUtility.HtmlEncode(c.ParentFirstName)),
                new XElement("ParentLastName", HttpUtility.HtmlEncode(c.ParentLastName)),
                new XElement("ParentEmail", c.ParentEmailID)
                ));
            List<ChildImport> _Result = new List<ChildImport>();
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spValiadteImportChildren", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Students", SqlDbType.Xml).Value = studentxml.ToString(); ;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        _SqlDataAdapter.Fill(_DataSet);

                        Status = (int)id.Value;
                        if (_DataSet.Tables.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                ChildImport _StudentImportExport = new ChildImport();
                                _StudentImportExport.ChildFirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                _StudentImportExport.ChildLastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                _StudentImportExport.ParentFirstName = HttpUtility.HtmlDecode(_DataRow["ParentFirstName"].ToString());
                                _StudentImportExport.ParentLastName = HttpUtility.HtmlDecode(_DataRow["ParentLastName"].ToString());
                                _StudentImportExport.Grade = _DataRow["Grade"].ToString();
                                _StudentImportExport.SubSection = HttpUtility.HtmlDecode(_DataRow["SubSection"].ToString());
                                _StudentImportExport.ParentEmailID = _DataRow["ParentEmail"].ToString();
                                _StudentImportExport.SubscriptionStartDate = Convert.ToDateTime(_DataRow["SubscriptionStartDate"]);
                                _StudentImportExport.SubscriptionEndDate = Convert.ToDateTime(_DataRow["SubscriptionEndDate"]);
                                _StudentImportExport.Status = Convert.ToBoolean(_DataRow["Status"]);
                                _StudentImportExport.RowNumber = Convert.ToInt32(_DataRow["RowNumber"]);
                                _StudentImportExport.SNO = Convert.ToInt32(_DataRow["SNO"]);
                                _StudentImportExport.GradeStatus = Convert.ToBoolean(_DataRow["GradeStatus"]);
                                _Result.Add(_StudentImportExport);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //InsertLog(_currentUserId, "ValidateImportStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _Result;
        }

        public UserStatus UpdateStudent(StudentProfileResult _Student)
        {
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateStudent", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = _Student.UserId;
                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_Student.FirstName);
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_Student.LastName);
                        command.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = _Student.Gender;
                        command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = _Student.DateOfBirth;
                        command.Parameters.Add("@SubSection", SqlDbType.NVarChar).Value = HttpUtility.HtmlEncode(_Student.SubSection);
                        command.Parameters.Add("@Grade", SqlDbType.NVarChar).Value = _Student.Grade;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            InsertLog(_currentUserId, "UpdateStudent", "Student updated", "Student with Id " + _Student.UserId+" is Updated", UserStatus.Sucess.ToString());
                            return UserStatus.Sucess;
                        }
                        else if (Status == 2)
                        {
                            InsertLog(_currentUserId, "UpdateStudent", "Error occured while updating student", "Error Occured while updating student with Id "+ _Student.UserId+". New data fro student is not unique.", UserStatus.Error.ToString());
                            return UserStatus.UserAlreadyRegistered;
                        }
                        else
                        {
                            InsertLog(_currentUserId, "UpdateStudent", "Error occured while updating student", "Error Occured while updating student with Id " + _Student.UserId, UserStatus.Error.ToString());
                            return UserStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UpdateStudent", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return UserStatus.Error;
            }
        }

        public UserStatus UserRegistrationEmail(int UserId)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUserRegistrationEmail", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;
                        if (_Status == 0)
                        {
                            InsertLog(_currentUserId, "UserRegistrationEmail", "User registration email", string.Format("Failed to send registartion email to userId {0} ", UserId), UserStatus.Error.ToString());
                            return UserStatus.Error;
                        }
                        else if (_Status == 1)
                        {
                            InsertLog(_currentUserId, "UserRegistrationEmail", "User registration email", string.Format("Registartion email has been sent to userId {0} ", UserId), UserStatus.Sucess.ToString());
                            return UserStatus.Sucess;
                        }
                        else
                        {
                            InsertLog(_currentUserId, "UserRegistrationEmail", "User registration email", string.Format("No user exists with userId {0} ", UserId), UserStatus.UserAccountNotExist.ToString());
                            return UserStatus.UserAccountNotExist;
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UserRegistrationEmail", string.Format("Error occured while sending registartion email to userId {0} ", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return UserStatus.Error;
            }
        }

        public UserResult AddElibAdmin(string Email, string MobileNo)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spAddElibAdmin", con))
                    {
                        string Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                        command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = MobileNo;
                        command.Parameters.Add("@Token", SqlDbType.NVarChar).Value = Token;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;

                        if (_Status == 1)
                        {
                            User _User = new User();
                            _User.Token = Token;
                            InsertLog(_currentUserId, "AddElibAdmin", "New Elib admin added", String.Format("New Elib admin added with Email={0} and Mobile={1}", Email, MobileNo), UserStatus.Sucess.ToString());
                            return new UserResult { User = _User, Status = UserStatus.Sucess };
                        }
                        else if (_Status == 0)
                        {
                            return new UserResult { Status = UserStatus.UserAlreadyRegistered };
                        }
                        else if (_Status == -1)
                        {
                            return new UserResult { Status = UserStatus.Error };
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "AddElibAdmin", "Failed to add new Elib admin", ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
            return new UserResult { Status = UserStatus.Error };
        }

        public User GetELibraryAdmin(int UserId)
        {
            User _Result = new User();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetELibraryAdmin", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _Result.Email = _DataRow["Email"].ToString();
                                    _Result.Username = _DataRow["UserName"].ToString();
                                    _Result.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _Result.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Result.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Result.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Result.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _Result.Gender = _DataRow["Gender"].ToString();
                                    _Result.Status = bool.Parse(_DataRow["Status"].ToString());
                                    _Result.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _Result.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                }
                            }
                            else
                            {
                                _Result = null;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetELibraryAdmin", string.Format("Error occured in getting elib admin {0} ", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetELibraryAdmin", string.Format("Error occured in getting elib admin {0} ", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }
        public SchoolData GetSchoolAdminDashboard(int UserId, int PageIndex, int PageSize, string SchoolUId)
        {
            SchoolData _Result = new SchoolData();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetSchoolAdminDashboard", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@SchoolUId", SqlDbType.NVarChar).Value = SchoolUId;
                        SqlParameter Status = new SqlParameter("@StatusApi", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            int _Status = (int)Status.Value;
                            if (_Status == 1)
                            {
                                if (_DataSet != null)
                                {
                                    if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                    {
                                        _Result.SchoolDetails.SchoolId = int.Parse(_DataSet.Tables[0].Rows[0]["SchoolId"].ToString());
                                        _Result.SchoolDetails.SchoolUId = _DataSet.Tables[0].Rows[0]["SchoolUId"].ToString();
                                        _Result.SchoolDetails.SchoolName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["Name"].ToString());
                                        _Result.SchoolDetails.AddressLine1 = _DataSet.Tables[0].Rows[0]["AddressLine1"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["AddressLine1"].ToString()) : "";
                                        _Result.SchoolDetails.AddressLine2 = _DataSet.Tables[0].Rows[0]["AddressLine2"] != DBNull.Value ? HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["AddressLine2"].ToString()) : "";
                                        _Result.SchoolDetails.State = _DataSet.Tables[0].Rows[0]["State"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["State"].ToString() : "";
                                        _Result.SchoolDetails.Country = _DataSet.Tables[0].Rows[0]["Country"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["Country"].ToString() : "";
                                        _Result.SchoolDetails.PinCode = _DataSet.Tables[0].Rows[0]["PinCode"] != DBNull.Value ? int.Parse(_DataSet.Tables[0].Rows[0]["PinCode"].ToString()) : 0;
                                        _Result.SchoolDetails.PrincipalName = HttpUtility.HtmlDecode(_DataSet.Tables[0].Rows[0]["PrincipalName"].ToString());
                                        _Result.SchoolDetails.PrincipalEmail = _DataSet.Tables[0].Rows[0]["PrincipalEmailID"].ToString();
                                        _Result.SchoolDetails.PhoneNumber = _DataSet.Tables[0].Rows[0]["PhoneNumber"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["PhoneNumber"].ToString() : "";
                                        _Result.SchoolDetails.IsActive = bool.Parse(_DataSet.Tables[0].Rows[0]["IsActive"].ToString());
                                        _Result.SchoolDetails.StudentCount = _DataSet.Tables[0].Rows[0]["StudentCount"] != DBNull.Value ? int.Parse(_DataSet.Tables[0].Rows[0]["StudentCount"].ToString()) : 0;
                                        _Result.SchoolDetails.City = _DataSet.Tables[0].Rows[0]["City"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["City"].ToString() : "";
                                        _Result.SchoolDetails.IsEmailVerified = _DataSet.Tables[0].Rows[0]["IsEmailVerified"] != DBNull.Value ? Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["IsEmailVerified"]) : false;
                                        _Result.SchoolDetails.IsTrashed = bool.Parse(_DataSet.Tables[0].Rows[0]["IsTrashed"].ToString());
                                    }
                                    if (_DataSet.Tables.Count > 1 && _DataSet.Tables[1].Rows.Count != 0)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                        {
                                            StudentModel _Student = new StudentModel();
                                            _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                            _Student.Email = _DataRow["Email"].ToString();
                                            _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                            _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                            _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                            _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                            _Student.ParentEmail = _DataRow["ParentEmail"].ToString();
                                            _Student.Grade = _DataRow["GradeName"].ToString();
                                            _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                            _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                            _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                            _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                            _Student.SchoolUId = _DataRow["SchoolUId"].ToString();
                                            _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                            _Result.Students.Items.Add(_Student);
                                        }
                                        _Result.Students.TotalItems = int.Parse(_DataSet.Tables[1].Rows[0]["TotalRows"].ToString());
                                        _Result.Students.PageSize = PageSize;
                                    }

                                    if (_DataSet.Tables.Count > 2)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                        {
                                            Grade _Grade = new Grade();
                                            _Grade.Id = int.Parse(_DataRow["Id"].ToString());
                                            _Grade.Name = _DataRow["Name"].ToString();
                                            _Result.Grades.Add(_Grade);
                                        }
                                    }
                                }
                                _Result.APIStatus = SchoolStatus.Sucess;
                            }
                            else
                                _Result.APIStatus = SchoolStatus.NoSchoolFound;
                        }
                        catch { }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetSchoolAdminDashboard", string.Format("Error occured in Get school({0}) admin({1}) dashboard", SchoolUId, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }
        public HelpItem CreateHelpItem(string UserMessage, int UserId)
        {
            HelpItem output = null;
            int Status = 0;
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spInsertHelpItem", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserMessage", SqlDbType.NVarChar).Value = UserMessage;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            Status = (int)id.Value;
                            if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0 && Status == 1)
                            {
                                output = new HelpItem();

                                output.UserEmail = _DataSet.Tables[0].Rows[0]["Email"].ToString();
                                output.ReferenceId = _DataSet.Tables[0].Rows[0]["ReferenceId"].ToString();
                                output.Query = _DataSet.Tables[0].Rows[0]["Query"].ToString();
                                output.CreatedOn = DateTime.Parse(_DataSet.Tables[0].Rows[0]["CreatedOn"].ToString());
                                output.StudentFirstName = _DataSet.Tables[0].Rows[0]["StudentFirstName"].ToString();
                                output.StudentLastName = _DataSet.Tables[0].Rows[0]["StudentLastName"].ToString();
                                output.Role = _DataSet.Tables[0].Rows[0]["Role"].ToString();
                            }
                            InsertLog(_currentUserId, "CreateHelpItem", "Create Help Item", string.Format("helpitem- {0} is created for user {1}", UserMessage, UserId), UserStatus.Sucess.ToString());
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "CreateHelpItem", string.Format("Error occured in helpitem- {0}  for user {1}", UserMessage, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            output = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "CreateHelpItem", string.Format("Error occured in helpitem- {0}  for user {1}", UserMessage, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                output = null;
            }
            return output;
        }

        public PagedList<StudentModel> GetStudentsOfSchoolAdmin(string SchoolUId, int PageSize, int PageIndex, string Grade)
        {
            int _Status = 0;
            PagedList<StudentModel> _Result = new PagedList<StudentModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudentsOfSchoolAdmin", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@Grade", SqlDbType.VarChar).Value = Grade;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            _Status = (int)id.Value;
                            if (_Status == 1)
                            {
                                if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        StudentModel _Student = new StudentModel();
                                        _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Student.Email = _DataRow["Email"].ToString();
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                        _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                        _Student.ParentEmail = _DataRow["ParentEmail"].ToString();
                                        _Student.Grade = _DataRow["GradeName"].ToString();
                                        _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                        _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                        _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                        _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                        _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                        _Result.Items.Add(_Student);
                                    }
                                    _Result.TotalItems = int.Parse(_DataSet.Tables[0].Rows[0]["TotalRows"].ToString());
                                    _Result.PageSize = PageSize;
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetStudentsOfSchoolAdmin", string.Format("Error occured in getting students of school {0} for schooladmin", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetStudentsOfSchoolAdmin", string.Format("Error occured in getting students of school {0} for schooladmin", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public RegistrationAndLogin GetRegistrationAndLoginReport(string UserType, string SchoolIds, int PageSize, int PageIndex)
        {
            RegistrationAndLogin _Result = new RegistrationAndLogin();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetRegistrationAndLoginReport", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserType", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(UserType) ? "" : UserType;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@SchoolIds", SqlDbType.VarChar).Value = string.IsNullOrEmpty(SchoolIds) ? "" : SchoolIds;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 2)
                            {
                                if (_DataSet.Tables.Count > 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        StudentModel _Student = new StudentModel();
                                        _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.Email = _DataRow["Email"].ToString();
                                        _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                        _Student.Username = _DataRow["Username"].ToString();
                                        _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                        _Student.Gender = _DataRow["Gender"].ToString();
                                        _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                        _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                        _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                        _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                        _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                        _Student.Result = "Created but Not Registered";
                                        _Student.Role = _DataRow["Role"].ToString();
                                        _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                        _Student.Grade = _DataRow["Grade"].ToString();
                                        _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                        _Result.Created.Items.Add(_Student);
                                    }
                                    _Result.Created.TotalItems = _DataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["TotalRows"]) : 0;
                                    _Result.Created.PageSize = PageSize;
                                    _Result.Created.PageIndex = PageIndex;
                                }

                                if (_DataSet.Tables.Count > 1)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                    {
                                        StudentModel _Student = new StudentModel();
                                        _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.Email = _DataRow["Email"].ToString();
                                        _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                        _Student.Username = _DataRow["Username"].ToString();
                                        _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                        _Student.Gender = _DataRow["Gender"].ToString();
                                        _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                        _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                        _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                        _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                        _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                        _Student.Result = "Registered but Never Logged";
                                        _Student.Role = _DataRow["Role"].ToString();
                                        _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                        _Student.Grade = _DataRow["Grade"].ToString();
                                        _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                        _Result.Registered.Items.Add(_Student);
                                    }
                                    _Result.Registered.TotalItems = _DataSet.Tables[1].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[1].Rows[0]["TotalRows"]) : 0;
                                    _Result.Registered.PageSize = PageSize;
                                    _Result.Registered.PageIndex = PageIndex;
                                }

                                if (_DataSet.Tables.Count > 2)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                    {
                                        StudentModel _Student = new StudentModel();
                                        _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.Email = _DataRow["Email"].ToString();
                                        _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                        _Student.Username = _DataRow["Username"].ToString();
                                        _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                        _Student.Gender = _DataRow["Gender"].ToString();
                                        _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                        _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                        _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                        _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                        _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                        _Student.Result = "Active";
                                        _Student.Role = _DataRow["Role"].ToString();
                                        _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                        _Student.Grade = _DataRow["Grade"].ToString();
                                        _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                        _Result.Active.Items.Add(_Student);
                                    }
                                    _Result.Active.TotalItems = _DataSet.Tables[2].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[2].Rows[0]["TotalRows"]) : 0;
                                    _Result.Active.PageSize = PageSize;
                                    _Result.Active.PageIndex = PageIndex;
                                }

                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetRegistrationAndLoginReport", string.Format("Error occured while getting report UserType={0}, SchoolIds={1}", UserType, SchoolIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetRegistrationAndLoginReport", string.Format("Error occured while getting report UserType={0}, SchoolIds={1}", UserType, SchoolIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public PagedList<StudentModel> GetRegistrationAndLoginReportList(string UserType, string SchoolIds, int PageSize, int PageIndex, string Type)
        {
            PagedList<StudentModel> _Result = new PagedList<StudentModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetRegistrationAndLoginReportList", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserType", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(UserType) ? "" : UserType;
                        command.Parameters.Add("@SchoolIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SchoolIds) ? "" : SchoolIds;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            if (_DataSet.Tables.Count > 0)
                            {
                                if (_DataSet.Tables.Count > 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        StudentModel _Student = new StudentModel();
                                        _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.Email = _DataRow["Email"].ToString();
                                        _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                        _Student.Username = _DataRow["Username"].ToString();
                                        _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                        _Student.Gender = _DataRow["Gender"].ToString();
                                        _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                        _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                        _Student.Role = _DataRow["Role"].ToString();
                                        _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                        _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                        _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                        if (Type == "created")
                                            _Student.Result = "Created but Not Registered";
                                        else if (Type == "registered")
                                            _Student.Result = "Registered but Never Logged";
                                        else
                                            _Student.Result = "Active";
                                        _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                        _Student.Grade = _DataRow["Grade"].ToString();
                                        _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                        _Result.Items.Add(_Student);
                                    }
                                    _Result.TotalItems = _DataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["TotalRows"]) : 0;
                                    _Result.PageSize = PageSize;
                                    _Result.PageIndex = PageIndex;
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetRegistrationAndLoginReportList", string.Format("Error occured while getting report UserType={0}, SchoolIds={1}", UserType, SchoolIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetRegistrationAndLoginReportList", string.Format("Error occured while getting report UserType={0}, SchoolIds={1}", UserType, SchoolIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public List<School> GetSchools(int UserId)
        {
            List<School> _Result = new List<School>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetSchools", con))
                    {
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                if (_DataSet.Tables.Count > 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        School _School = new School();
                                        _School.SchoolId = Convert.ToInt32(_DataRow["SchoolId"]);
                                        _School.SchoolName = HttpUtility.HtmlDecode(_DataRow["Name"].ToString());
                                        _Result.Add(_School);
                                    }
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetSchools", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetSchools", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report6 GetReport6Filter()
        {
            Report6 _Result = new Report6();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport6Filter", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 1)
                            {

                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    SubSection _SubSection = new SubSection();
                                    _SubSection.Id = int.Parse(_DataRow["Id"].ToString());
                                    _SubSection.Name = _DataRow["Name"].ToString();
                                    _Result.Subsections.Add(_SubSection);
                                }
                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    Language _Language = new Language();
                                    _Language.Id = int.Parse(_DataRow["Id"].ToString());
                                    _Language.Name = _DataRow["Name"].ToString();
                                    _Result.Languages.Add(_Language);
                                }

                                _Result.Status = Report6Status.Filter;
                            }
                            else
                            {
                                _Result.Status = Report6Status.Error;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport6Filter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = Report6Status.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport6Filter", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = Report6Status.Error;
            }
            return _Result;
        }

        public Report6 GetReport6(string SubsectionIds, string LanguageIds)
        {
            Report6 _Result = new Report6();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport6", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LanguageIds", SqlDbType.VarChar).Value = LanguageIds;
                        command.Parameters.Add("@SubsectionIds", SqlDbType.VarChar).Value = SubsectionIds;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {

                                _Result._ReportSubsections = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                              select new ReportSubsections
                                                              {
                                                                  Id = dr.Field<int>("SubsectionId"),
                                                                  Name = dr.Field<string>("SubsectionName"),
                                                              }).ToList().DistinctBy(x => x.Id).ToList();
                                for (int i = 0; i < _Result._ReportSubsections.Count; i++)
                                {
                                    _Result._ReportSubsections[i]._ReportLanguages = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                                      where dr.Field<int>("SubsectionId") == _Result._ReportSubsections[i].Id
                                                                                      select new ReportLanguages
                                                                                      {
                                                                                          Id = dr.Field<int>("LanguageId"),
                                                                                          Name = dr.Field<string>("LanguageName"),
                                                                                      }).ToList().DistinctBy(x => x.Id).ToList();
                                }

                                for (int i = 0; i < _Result._ReportSubsections.Count; i++)
                                {
                                    for (int j = 0; j < _Result._ReportSubsections[i]._ReportLanguages.Count; j++)
                                    {
                                        _Result._ReportSubsections[i]._ReportLanguages[j].Books = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                                                   where dr.Field<int>("SubsectionId") == _Result._ReportSubsections[i].Id && dr.Field<int>("LanguageId") == _Result._ReportSubsections[i]._ReportLanguages[j].Id
                                                                                                   select new Book
                                                                                                   {
                                                                                                       BookId = dr.Field<int>("BookId"),
                                                                                                       Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                                                                                       Author = HttpUtility.HtmlDecode(dr.Field<string>("Author")),
                                                                                                       Translator = HttpUtility.HtmlDecode(dr.Field<string>("Translator")),
                                                                                                       Publisher = HttpUtility.HtmlDecode(dr.Field<string>("Publisher")),
                                                                                                       Illustrator = HttpUtility.HtmlDecode(dr.Field<string>("Illustrator")),
                                                                                                       HasActivity = dr.Field<bool>("HasActivity"),
                                                                                                       HasAnimation = dr.Field<bool>("HasAnimation"),
                                                                                                       HasReadAloud = dr.Field<bool>("HasReadAloud"),
                                                                                                       ShortDescription = HttpUtility.HtmlDecode(dr.Field<string>("ShortDescription")),
                                                                                                       Language = dr.Field<string>("Language"),
                                                                                                       SubSection = dr.Field<string>("SubSection"),
                                                                                                       Genre = dr.Field<string>("Genre"),
                                                                                                       Type = dr.Field<string>("Type"),
                                                                                                       Rating = dr.IsNull("Rating") ? 0 : Convert.ToSingle(dr.Field<string>("Rating")),
                                                                                                       ViewMode = dr.Field<string>("ViewMode"),
                                                                                                       Thumbnail1 = dr.Field<string>("Thumbnail1"),
                                                                                                   }).ToList();
                                    }
                                }
                                _Result.Status = Report6Status.Report;
                            }
                            else
                            {
                                _Result.Status = Report6Status.Error;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport6", string.Format("Error occured while getting report SubsectionIds={0}, LanguageIds={1}", SubsectionIds, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = Report6Status.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport6", string.Format("Error occured while getting report SubsectionIds={0}, LanguageIds={1}", SubsectionIds, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = Report6Status.Error;
            }
            return _Result;
        }

        public List<Book> GetReport6ForExport(string SubsectionIds, string LanguageIds)
        {
            List<Book> _Result = new List<Book>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport6ForExport", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LanguageIds", SqlDbType.VarChar).Value = LanguageIds;
                        command.Parameters.Add("@SubsectionIds", SqlDbType.VarChar).Value = SubsectionIds;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result = (from dr in _DataSet.Tables[0].AsEnumerable()
                                           select new Book
                                           {
                                               BookId = dr.Field<int>("BookId"),
                                               Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                               Author = HttpUtility.HtmlDecode(dr.Field<string>("Author")),
                                               Translator = HttpUtility.HtmlDecode(dr.Field<string>("Translator")),
                                               Publisher = HttpUtility.HtmlDecode(dr.Field<string>("Publisher")),
                                               Illustrator = HttpUtility.HtmlDecode(dr.Field<string>("Illustrator")),
                                               HasActivity = dr.Field<bool>("HasActivity"),
                                               HasAnimation = dr.Field<bool>("HasAnimation"),
                                               HasReadAloud = dr.Field<bool>("HasReadAloud"),
                                               ShortDescription = HttpUtility.HtmlDecode(dr.Field<string>("ShortDescription")),
                                               Language = dr.Field<string>("Language"),
                                               SubSection = dr.Field<string>("SubSection"),
                                               Genre = dr.Field<string>("Genre"),
                                               Type = dr.Field<string>("Type"),
                                               Rating = dr.IsNull("Rating") ? 0 : Convert.ToSingle(dr.Field<string>("Rating")),
                                           }).ToList();
                            }
                            else
                            {
                                return null;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport6ForExport", string.Format("Error occured while exporting report SubsectionIds={0}, LanguageIds={1}", SubsectionIds, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport6ForExport", string.Format("Error occured while exporting report SubsectionIds={0}, LanguageIds={1}", SubsectionIds, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _Result;
        }

        public List<StudentImportExport> ValidateBulkUpdateStudents(List<StudentImportExport> Students, string SchoolUId)
        {
            var studentxml = new XElement("Students",
            from c in Students
            select new XElement("Student",
                new XElement("SNO", c.SNO),
                new XElement("FirstName", HttpUtility.HtmlDecode(c.FirstName)),
                new XElement("LastName", HttpUtility.HtmlDecode(c.LastName)),
                new XElement("RollNo", c.RollNo),
                new XElement("Grade", c.Grade),
                new XElement("SubSection", HttpUtility.HtmlEncode(c.SubSection)),
                new XElement("SubscriptionStartDate", c.SubscriptionStartDate),
                new XElement("SubscriptionEndDate", c.SubscriptionEndDate),
                new XElement("IsRenew", c.IsRenew.ToLower()),
                new XElement("UniqueId", c.UniqueId)
                ));
            List<StudentImportExport> _Result = new List<StudentImportExport>();
            int Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spValiadteBulkUpdateStudents", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Students", SqlDbType.Xml).Value = studentxml.ToString();
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        _SqlDataAdapter.Fill(_DataSet);

                        Status = (int)id.Value;
                        if (_DataSet.Tables.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                StudentImportExport _StudentImportExport = new StudentImportExport();
                                _StudentImportExport.SNO = Convert.ToInt32(_DataRow["SNO"].ToString());
                                _StudentImportExport.RowNumber = Convert.ToInt32(_DataRow["RowNumber"].ToString());
                                _StudentImportExport.StudentStatus = Convert.ToBoolean(_DataRow["StudentStatus"]);
                                _StudentImportExport.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                _StudentImportExport.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                _StudentImportExport.Grade = _DataRow["Grade"].ToString();
                                _StudentImportExport.SubSection = HttpUtility.HtmlDecode(_DataRow["SubSection"].ToString());
                                _StudentImportExport.RollNo = _DataRow["RollNo"].ToString();
                                _StudentImportExport.SubscriptionStartDate = Convert.ToDateTime(_DataRow["SubscriptionStartDate"]);
                                _StudentImportExport.SubscriptionEndDate = Convert.ToDateTime(_DataRow["SubscriptionEndDate"]);
                                _StudentImportExport.Status = Convert.ToBoolean(_DataRow["Status"]);
                                _StudentImportExport.IsRenew = _DataRow["IsRenew"].ToString();
                                _StudentImportExport.GradeStatus = Convert.ToBoolean(_DataRow["GradeStatus"]);
                                _StudentImportExport.UniqueId = _DataRow["UniqueId"].ToString();
                                _StudentImportExport.IsAlreadyExists = Convert.ToBoolean(_DataRow["IsAlreadyExists"]);
                                _Result.Add(_StudentImportExport);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "ValidateBulkUpdateStudents", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return null;
            }
            return _Result;
        }

        public List<StudentImportExport> GetStudentsOfSchoolForExport(string SchoolUId, int PageSize, int PageIndex, string SearchTxt, string Grade)
        {
            int _Status = 0;
            List<StudentImportExport> _Result = new List<StudentImportExport>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudentsOfSchool", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@Grade", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Grade) ? "" : Grade;
                        command.Parameters.Add("@SearchTxt", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SearchTxt) ? string.Empty : SearchTxt;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            _Status = (int)id.Value;
                            if (_Status == 1)
                            {
                                if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        StudentImportExport _Student = new StudentImportExport();
                                        _Student.SNO = int.Parse(_DataRow["SNO"].ToString());
                                        _Student.ParentEmail = _DataRow["Email"].ToString();
                                        _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                        _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                        _Student.RollNo = _DataRow["RollNo"].ToString();
                                        _Student.SubSection = HttpUtility.HtmlDecode(_DataRow["Section"].ToString());
                                        _Student.Grade = _DataRow["GradeName"].ToString();
                                        _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                        _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                        _Student.UniqueId = _DataRow["UniqueId"].ToString();
                                        _Result.Add(_Student);
                                    }
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetStudentsOfSchoolForExport", string.Format("Error occured while getting student for export schooluid={0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetStudentsOfSchoolForExport", string.Format("Error occured while getting student for export schooluid={0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report7FilterModel GetReport7Fillters()
        {

            Report7FilterModel _Result = new Report7FilterModel();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport7Fillters", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 1)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.Cities.Add(_DataRow["City"].ToString());
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    School _school = new School();
                                    _school.SchoolId = Convert.ToInt32(_DataRow["SchoolId"]);
                                    _school.SchoolName = HttpUtility.HtmlDecode(_DataRow["Name"].ToString());
                                    _school.City = _DataRow["City"].ToString();
                                    _Result.Schools.Add(_school);
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport7Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport7Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report7Model GetReport7(DateTime StartDate, DateTime EndDate, string SchoolIds, string City)
        {
            Report7Model _Result = new Report7Model();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport7", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@SchoolIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SchoolIds) ? "" : SchoolIds; ;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 1)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.Cities.Add(_DataRow["City"].ToString());
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    School _school = new School();
                                    _school.SchoolId = Convert.ToInt32(_DataRow["SchoolId"]);
                                    _school.SchoolName = HttpUtility.HtmlDecode(_DataRow["Name"].ToString());
                                    _school.City = _DataRow["City"].ToString();
                                    _Result.Schools.Add(_school);
                                }
                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    UsageData _UsageData = new UsageData();
                                    _UsageData.Percent = Convert.ToSingle(_DataRow["per"]);
                                    _UsageData.Name = _DataRow["Name"].ToString();
                                    _Result.UsageModel.Palteforms.Add(_UsageData);
                                }
                                foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                {
                                    UsageData _UsageData = new UsageData();
                                    _UsageData.Percent = Convert.ToSingle(_DataRow["per"]);
                                    _UsageData.Name = _DataRow["Name"].ToString();
                                    _Result.UsageModel.Environment.Add(_UsageData);
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport7", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, SchoolIds={2}, City={3}", StartDate, EndDate, SchoolIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport7", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, SchoolIds={2}, City={3}", StartDate, EndDate, SchoolIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public List<Report7ExportModel> GetReport7Export(DateTime StartDate, DateTime EndDate, string SchoolIds, string City)
        {
            List<Report7ExportModel> _Result = new List<Report7ExportModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport7Export", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@SchoolIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(SchoolIds) ? "" : SchoolIds; ;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    Report7ExportModel _Report7ExportModel = new Report7ExportModel();
                                    _Report7ExportModel.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Report7ExportModel.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Report7ExportModel.UserName = _DataRow["Username"].ToString();
                                    _Report7ExportModel.ActivityType = _DataRow["ActivityType"].ToString();
                                    _Report7ExportModel.ActivityDate = Convert.ToDateTime(_DataRow["ActivityDate"]);
                                    _Report7ExportModel.Platform = _DataRow["Platform"].ToString();
                                    _Report7ExportModel.Environment = _DataRow["Environment"].ToString();
                                    _Report7ExportModel.ActivityDuration = Convert.ToInt32(_DataRow["ActivityDuration"]);
                                    _Report7ExportModel.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                    _Report7ExportModel.Grade = _DataRow["Grade"].ToString();
                                    _Result.Add(_Report7ExportModel);
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport7Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, SchoolIds={2}, City={3}", StartDate, EndDate, SchoolIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport7Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, SchoolIds={2}, City={3}", StartDate, EndDate, SchoolIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report2FilterModel GetReport2Fillters(int UserId)
        {

            Report2FilterModel _Result = new Report2FilterModel();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport2Fillters", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 2)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.Cities.Add(_DataRow["City"].ToString());
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    School _school = new School();
                                    _school.SchoolId = Convert.ToInt32(_DataRow["SchoolId"]);
                                    _school.SchoolName = HttpUtility.HtmlDecode(_DataRow["Name"].ToString());
                                    _school.City = _DataRow["City"].ToString();
                                    _Result.Schools.Add(_school);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    Grade _grade = new Grade();
                                    _grade.Id = Convert.ToInt32(_DataRow["Id"]);
                                    _grade.Name = _DataRow["Name"].ToString();
                                    _Result.Grades.Add(_grade);
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport2Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport2Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report3FilterModel GetReport3Fillters(int UserId)
        {

            Report3FilterModel _Result = new Report3FilterModel();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport3Fillters", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 3)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.Cities.Add(_DataRow["City"].ToString());
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    SubSection _SubSection = new SubSection();
                                    _SubSection.Id = Convert.ToInt32(_DataRow["Id"]);
                                    _SubSection.Name = _DataRow["Name"].ToString();
                                    _Result.SubSections.Add(_SubSection);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    Language _Language = new Language();
                                    _Language.Id = Convert.ToInt32(_DataRow["Id"]);
                                    _Language.Name = _DataRow["Name"].ToString();
                                    _Result.Languages.Add(_Language);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                {
                                    Book _Book = new Book();
                                    _Book.BookId = Convert.ToInt32(_DataRow["BookId"]);
                                    _Book.Title = HttpUtility.HtmlDecode(_DataRow["Title"].ToString());
                                    _Book.SubSection = _DataRow["SubSection"].ToString();
                                    _Book.Language = _DataRow["Language"].ToString();
                                    _Result.Books.Add(_Book);
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport3Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport3Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report4FilterModel GetReport4Fillters()
        {

            Report4FilterModel _Result = new Report4FilterModel();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport4Fillters", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 3)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.Cities.Add(_DataRow["City"].ToString());
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                {
                                    SubSection _SubSection = new SubSection();
                                    _SubSection.Id = Convert.ToInt32(_DataRow["Id"]);
                                    _SubSection.Name = _DataRow["Name"].ToString();
                                    _Result.SubSections.Add(_SubSection);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    Language _Language = new Language();
                                    _Language.Id = Convert.ToInt32(_DataRow["Id"]);
                                    _Language.Name = _DataRow["Name"].ToString();
                                    _Result.Languages.Add(_Language);
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[3].Rows)
                                {
                                    Report4Publisher _publisher = new Report4Publisher();
                                    _publisher.Publisher = HttpUtility.HtmlDecode(_DataRow["Publisher"].ToString());
                                    _publisher.SubSection = HttpUtility.HtmlDecode(_DataRow["SubSection"].ToString());
                                    _publisher.Language = _DataRow["Language"].ToString();
                                    _Result.Publishers.Add(_publisher);
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport4Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport4Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report3 GetReport3(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string BookIds, string City)
        {
            Report3 _Result = new Report3();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport3", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LanguageIds", SqlDbType.VarChar).Value = LanguageIds;
                        command.Parameters.Add("@SubsectionIds", SqlDbType.VarChar).Value = SubsectionIds;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@BookIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(BookIds) ? "" : BookIds;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {

                                _Result.SubSection = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                      select new Report3SubSections
                                                      {
                                                          Id = dr.Field<int>("SubsectionId"),
                                                          Name = dr.Field<string>("SubsectionName"),
                                                      }).ToList().DistinctBy(x => x.Id).OrderBy(y => y.Name).ToList();

                                for (int i = 0; i < _Result.SubSection.Count; i++)
                                {
                                    _Result.SubSection[i].Language = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                      where dr.Field<int>("SubsectionId") == _Result.SubSection[i].Id
                                                                      select new Report3Languages
                                                                      {
                                                                          Id = dr.Field<int>("LanguageId"),
                                                                          Name = dr.Field<string>("LanguageName"),
                                                                      }).ToList().DistinctBy(x => x.Id).OrderBy(y => y.Name).ToList();
                                }

                                for (int i = 0; i < _Result.SubSection.Count; i++)
                                {
                                    for (int j = 0; j < _Result.SubSection[i].Language.Count; j++)
                                    {
                                        _Result.SubSection[i].Language[j].ReportBook = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                                        where dr.Field<int>("SubsectionId") == _Result.SubSection[i].Id && dr.Field<int>("LanguageId") == _Result.SubSection[i].Language[j].Id
                                                                                        select new ReportBook
                                                                                        {
                                                                                            BookId = dr.Field<int>("BookId"),
                                                                                            Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                                                                            Thumbnail = dr.Field<string>("Thumbnail1"),
                                                                                            ViewMode = dr.Field<string>("ViewMode"),
                                                                                            SubSection = dr.Field<string>("SubSection"),
                                                                                            AvgRating = dr.IsNull("Rating") ? 0 : Convert.ToSingle(dr.Field<string>("Rating")),
                                                                                            AvgReadingTime = dr.IsNull("AvgReadingTime") ? 0 : Convert.ToSingle(dr.Field<string>("AvgReadingTime")),
                                                                                            AvgActivityTime = dr.IsNull("AvgActivityTime") ? 0 : Convert.ToSingle(dr.Field<string>("AvgActivityTime")),
                                                                                        }).ToList();
                                    }
                                }


                                for (int i = 0; i < _Result.SubSection.Count; i++)
                                {
                                    for (int j = 0; j < _Result.SubSection[i].Language.Count; j++)
                                    {
                                        for (int k = 0; k < _Result.SubSection[i].Language[j].ReportBook.Count; k++)
                                        {
                                            // fill Raed
                                            int val = _DataSet.Tables[1].Compute("SUM(Count)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Compute("SUM(Count)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                            _Result.SubSection[i].Language[j].ReportBook[k].TotalRead = val;

                                            _Result.SubSection[i].Language[j].ReportBook[k].Reading = (from dr in _DataSet.Tables[1].AsEnumerable()
                                                                                                       where dr.Field<int>("BookId") == _Result.SubSection[i].Language[j].ReportBook[k].BookId
                                                                                                       select new PieChart
                                                                                                       {
                                                                                                           Name = dr.Field<string>("GradeName"),
                                                                                                           Count = dr.Field<int>("Count"),
                                                                                                       }).ToList();
                                            //fill avg rating

                                            _Result.SubSection[i].Language[j].ReportBook[k].Rating = (from dr in _DataSet.Tables[2].AsEnumerable()
                                                                                                      where dr.Field<int>("BookId") == _Result.SubSection[i].Language[j].ReportBook[k].BookId
                                                                                                      select new PieChart
                                                                                                      {
                                                                                                          Name = dr.Field<string>("Name"),
                                                                                                          Count = dr.Field<int>("Count"),
                                                                                                      }).ToList();

                                            val = _DataSet.Tables[2].Compute("SUM(Count)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[2].Compute("SUM(Count)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                            _Result.SubSection[i].Language[j].ReportBook[k].TotalRating = val;

                                            //fill activity
                                            val = _DataSet.Tables[3].Compute("SUM(Count)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[3].Compute("SUM(Count)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                            _Result.SubSection[i].Language[j].ReportBook[k].TotalActivity = val;


                                            _Result.SubSection[i].Language[j].ReportBook[k].Activity = (from dr in _DataSet.Tables[3].AsEnumerable()
                                                                                                        where dr.Field<int>("BookId") == _Result.SubSection[i].Language[j].ReportBook[k].BookId
                                                                                                        select new PieChart
                                                                                                        {
                                                                                                            Name = dr.Field<string>("Name"),
                                                                                                            Count = dr.Field<int>("Count"),
                                                                                                        }).ToList();
                                            val = _DataSet.Tables[4].Compute("SUM(TotalCompleted)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[4].Compute("SUM(TotalCompleted)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                            _Result.SubSection[i].Language[j].ReportBook[k].ReadComplete = val;
                                            val = _DataSet.Tables[4].Compute("SUM(TotalRead)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[4].Compute("SUM(TotalRead)", "BookId=" + _Result.SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                            _Result.SubSection[i].Language[j].ReportBook[k].TotalRead = val;

                                        }
                                    }
                                }
                            }

                        }

                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport3", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, BookIds={2}, City={3}, LanguageIds={4}", StartDate, EndDate, BookIds, City, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport3", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, BookIds={2}, City={3}, LanguageIds={4}", StartDate, EndDate, BookIds, City, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public Report4 GetReport4(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string Publisher, string City)
        {
            Report4 _Result = new Report4();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport4", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LanguageIds", SqlDbType.VarChar).Value = LanguageIds;
                        command.Parameters.Add("@SubsectionIds", SqlDbType.VarChar).Value = SubsectionIds;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@Publisher", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(Publisher) ? "" : HttpUtility.HtmlEncode(Publisher);
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result.Publisher = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                     select new Report4Publishers
                                                     {
                                                         Name = HttpUtility.HtmlDecode(dr.Field<string>("Publisher")),
                                                     }).ToList().DistinctBy(x => x.Name).OrderBy(y => y.Name).ToList();

                                for (int i = 0; i < _Result.Publisher.Count; i++)
                                {
                                    _Result.Publisher[i].SubSection = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                       where HttpUtility.HtmlDecode(dr.Field<string>("Publisher")) == _Result.Publisher[i].Name
                                                                       select new Report3SubSections
                                                                       {
                                                                           Id = dr.Field<int>("SubSectionId"),
                                                                           Name = HttpUtility.HtmlDecode(dr.Field<string>("SubSectionName")),
                                                                       }).ToList().DistinctBy(x => x.Name).OrderBy(y => y.Name).ToList();
                                }

                                for (int i = 0; i < _Result.Publisher.Count; i++)
                                {
                                    for (int j = 0; j < _Result.Publisher[i].SubSection.Count; j++)
                                    {
                                        _Result.Publisher[i].SubSection[j].Language = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                                       where dr.Field<int>("SubSectionId") == _Result.Publisher[i].SubSection[j].Id
                                                                                       select new Report3Languages
                                                                                       {
                                                                                           Id = dr.Field<int>("LanguageId"),
                                                                                           Name = dr.Field<string>("LanguageName"),
                                                                                       }).ToList().DistinctBy(x => x.Id).ToList();
                                    }
                                }
                                for (int k = 0; k < _Result.Publisher.Count; k++)
                                {
                                    for (int i = 0; i < _Result.Publisher[k].SubSection.Count; i++)
                                    {
                                        for (int j = 0; j < _Result.Publisher[k].SubSection[i].Language.Count; j++)
                                        {
                                            _Result.Publisher[k].SubSection[i].Language[j].ReportBook = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                                                         where dr.Field<int>("SubsectionId") == _Result.Publisher[k].SubSection[i].Id && dr.Field<int>("LanguageId") == _Result.Publisher[k].SubSection[i].Language[j].Id && dr.Field<string>("Publisher") == _Result.Publisher[k].Name
                                                                                                         select new ReportBook
                                                                                                         {
                                                                                                             BookId = dr.Field<int>("BookId"),
                                                                                                             Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                                                                                             Thumbnail = dr.Field<string>("Thumbnail1"),
                                                                                                             ViewMode = dr.Field<string>("ViewMode"),
                                                                                                             SubSection = dr.Field<string>("SubSection"),
                                                                                                             AvgRating = dr.IsNull("Rating") ? 0 : Convert.ToSingle(dr.Field<string>("Rating")),
                                                                                                             AvgReadingTime = dr.IsNull("AvgReadingTime") ? 0 : Convert.ToSingle(dr.Field<string>("AvgReadingTime")),
                                                                                                         }).ToList();
                                        }
                                    }
                                }
                                for (int l = 0; l < _Result.Publisher.Count; l++)
                                {
                                    for (int i = 0; i < _Result.Publisher[l].SubSection.Count; i++)
                                    {
                                        for (int j = 0; j < _Result.Publisher[l].SubSection[i].Language.Count; j++)
                                        {
                                            for (int k = 0; k < _Result.Publisher[l].SubSection[i].Language[j].ReportBook.Count; k++)
                                            {
                                                // fill Raed
                                                int val = _DataSet.Tables[1].Compute("SUM(Count)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Compute("SUM(Count)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                                _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].TotalRead = val;

                                                _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].Reading = (from dr in _DataSet.Tables[1].AsEnumerable()
                                                                                                                        where dr.Field<int>("BookId") == _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId
                                                                                                                        select new PieChart
                                                                                                                        {
                                                                                                                            Name = dr.Field<string>("GradeName"),
                                                                                                                            Count = dr.Field<int>("Count"),
                                                                                                                        }).ToList();
                                                //fill avg rating

                                                _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].Rating = (from dr in _DataSet.Tables[2].AsEnumerable()
                                                                                                                       where dr.Field<int>("BookId") == _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId
                                                                                                                       select new PieChart
                                                                                                                       {
                                                                                                                           Name = dr.Field<string>("Name"),
                                                                                                                           Count = dr.Field<int>("Count"),
                                                                                                                       }).ToList();

                                                val = _DataSet.Tables[2].Compute("SUM(Count)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[2].Compute("SUM(Count)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                                _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].TotalRating = val;

                                                val = _DataSet.Tables[3].Compute("SUM(TotalCompleted)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[3].Compute("SUM(TotalCompleted)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                                _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].ReadComplete = val;
                                                val = _DataSet.Tables[3].Compute("SUM(TotalRead)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[3].Compute("SUM(TotalRead)", "BookId=" + _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].BookId)) : 0;
                                                _Result.Publisher[l].SubSection[i].Language[j].ReportBook[k].TotalRead = val;

                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport4", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, Publisher={2}, City={3}, LanguageIds={4}", StartDate, EndDate, Publisher, City, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport4", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, Publisher={2}, City={3}, LanguageIds={4}", StartDate, EndDate, Publisher, City, LanguageIds), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public List<StudentModel> GetReport8Fillters(int UserId)
        {
            List<StudentModel> _Result = new List<StudentModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport8Fillters", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);

                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                _Result.Add(new StudentModel() { UserId = Convert.ToInt32(_DataRow["UserId"]), FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString()), LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString()) });
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport8Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport8Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public List<Report8> GetReport8(DateTime StartDate, DateTime EndDate, string StudentIds, int UserId)
        {
            List<Report8> _Result = new List<Report8>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport8", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@StudentIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(StudentIds) ? "" : StudentIds;
                        command.Parameters.Add("@ParentId", SqlDbType.NVarChar).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow item in _DataSet.Tables[0].Rows)
                                {
                                    Report8 _Report8 = new Report8();
                                    _Report8.UserId = Convert.ToInt32(item["UserId"]);
                                    _Report8.FirstName = HttpUtility.HtmlDecode(item["FirstName"].ToString());
                                    _Report8.LastName = HttpUtility.HtmlDecode(item["LastName"].ToString());

                                    _Report8.Books = (from dr in _DataSet.Tables[1].AsEnumerable()
                                                      where dr.Field<int>("UserId") == _Report8.UserId
                                                      select new Report8Book
                                                      {
                                                          BookId = dr.Field<int>("BookId"),
                                                          Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                                          IsActivityDone = dr.Field<int>("IsActivityDone") == 1 ? true : false,
                                                          IsRated = dr.Field<int>("IsRated") == 1 ? true : false,
                                                          TimeSpent = dr.IsNull("TimeSpent") ? 0 : dr.Field<int>("TimeSpent"),
                                                      }).ToList();

                                    _Report8.Activities = (from dr in _DataSet.Tables[2].AsEnumerable()
                                                           where dr.Field<int>("UserId") == _Report8.UserId
                                                           select new PieChart
                                                           {
                                                               Name = dr.Field<string>("Activity"),
                                                               Count = dr.Field<int>("Total"),
                                                           }).ToList();
                                    _Report8.TotalBookRated = _DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsRated=1") != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsRated=1")) : 0;
                                    _Report8.TotalActivityCompleted = _DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsActivityDone=1") != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsActivityDone=1")) : 0;
                                    _Report8.TimeSpent = _DataSet.Tables[2].Compute("Sum(Total)", "UserId=" + _Report8.UserId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[2].Compute("Sum(Total)", "UserId=" + _Report8.UserId)) : 0;
                                    _Result.Add(_Report8);
                                }

                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport8", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, StudentIds={2}, UserId={3}", StartDate, EndDate, StudentIds, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport8", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, StudentIds={2}, UserId={3}", StartDate, EndDate, StudentIds, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public List<Report8Export> GetReport8Export(DateTime StartDate, DateTime EndDate, string StudentIds, int UserId)
        {
            List<Report8Export> _Result = new List<Report8Export>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport8Export", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@StudentIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(StudentIds) ? "" : StudentIds;
                        command.Parameters.Add("@ParentId", SqlDbType.NVarChar).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {

                                _Result = (from dr in _DataSet.Tables[0].AsEnumerable()
                                           select new Report8Export
                                           {
                                               Title = dr.Field<string>("Title"),
                                               ActivityDone = dr.Field<string>("ActivityDone"),
                                               Rated = dr.Field<string>("Rated"),
                                               FirstName = HttpUtility.HtmlDecode(dr.Field<string>("FirstName")),
                                               LastName = HttpUtility.HtmlDecode(dr.Field<string>("LastName")),
                                               ReadingTime = (float)dr.Field<double>("ReadingTime"),
                                               ActivityTime = (float)dr.Field<double>("ActivityTime"),
                                               BrowsingTime = (float)dr.Field<double>("BrowsingTime"),
                                           }).ToList();
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport8Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, StudentIds={2}, UserId={3}", StartDate, EndDate, StudentIds, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport8Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, StudentIds={2}, UserId={3}", StartDate, EndDate, StudentIds, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public Report2 GetReport2(DateTime StartDate, DateTime EndDate, string SchoolIds, string GradeIds, string City)
        {
            Report2 _Result = new Report2();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport2", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolIds", SqlDbType.VarChar).Value = string.IsNullOrEmpty(SchoolIds) ? "" : SchoolIds; ;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@GradeIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(GradeIds) ? "" : GradeIds;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {

                                _Result.City = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                where !dr.IsNull("City")
                                                select new Report2City
                                                {
                                                    Name = dr.Field<string>("City"),
                                                }).ToList().DistinctBy(x => x.Name).OrderBy(y => y.Name).ToList();


                                for (int i = 0; i < _Result.City.Count; i++)
                                {
                                    _Result.City[i].School = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                              where dr.Field<string>("City") == _Result.City[i].Name
                                                              select new Report2School
                                                              {
                                                                  SchoolId = dr.Field<int>("SchoolId"),
                                                                  SchoolName = HttpUtility.HtmlDecode(dr.Field<string>("SchoolName")),
                                                              }).ToList().DistinctBy(x => x.SchoolId).OrderBy(y => y.SchoolName).ToList();
                                }

                                for (int i = 0; i < _Result.City.Count; i++)
                                {
                                    for (int j = 0; j < _Result.City[i].School.Count; j++)
                                    {
                                        _Result.City[i].School[j].Grade = (from dr in _DataSet.Tables[1].AsEnumerable()
                                                                           where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                           select new Grade
                                                                           {
                                                                               Id = dr.Field<int>("GradeId"),
                                                                               Name = dr.Field<string>("Grade"),
                                                                           }).OrderBy(y => y.Name).ToList();

                                        _Result.City[i].School[j].Section = _DataSet.Tables[10].AsEnumerable()
                                                            .Where(x => x.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId && _Result.City[i].School[j].Grade[0].Id == x.Field<int>("GradeId")).Select(x => x.Field<string>("Section")).ToList();
                                                            
                                        int val = _DataSet.Tables[1].Compute("SUM(Total)", "SchoolId=" + _Result.City[i].School[j].SchoolId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Compute("SUM(Total)", "SchoolId=" + _Result.City[i].School[j].SchoolId)) : 0;
                                        _Result.City[i].School[j].OverAllTotalStudents = val;
                                        _Result.City[i].School[j].OverAllStudents = (from dr in _DataSet.Tables[1].AsEnumerable()
                                                                                     where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                                     select new PieChart
                                                                                     {
                                                                                         Name = dr.Field<string>("Grade"),
                                                                                         Count = dr.Field<int>("Total"),
                                                                                     }).OrderBy(y => y.Name).ToList();
                                        val = _DataSet.Tables[2].Compute("SUM(TimeSpent)", "SchoolId=" + _Result.City[i].School[j].SchoolId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[2].Compute("SUM(TimeSpent)", "SchoolId=" + _Result.City[i].School[j].SchoolId)) : 0;
                                        _Result.City[i].School[j].OverAllTimeSpent = val;
                                        _Result.City[i].School[j].OverAllActivitiesGrade = (from dr in _DataSet.Tables[2].AsEnumerable()
                                                                                            where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                                            select new PieChart
                                                                                            {
                                                                                                Name = dr.Field<string>("Activity"),
                                                                                                Count = dr.Field<int>("TimeSpent"),
                                                                                            }).OrderBy(y => y.Name).ToList();
                                        val = _DataSet.Tables[4].Compute("SUM(TotalRead)", "SchoolId=" + _Result.City[i].School[j].SchoolId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[4].Compute("SUM(TotalRead)", "SchoolId=" + _Result.City[i].School[j].SchoolId)) : 0;
                                        _Result.City[i].School[j].OverAllAvgBookRead = val;

                                        _Result.City[i].School[j].GradeWise = (from dr in _DataSet.Tables[3].AsEnumerable()
                                                                               where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                               select new Report2GradeSection
                                                                               {
                                                                                   Id = dr.Field<int>("GradeId"),
                                                                                   Name = HttpUtility.HtmlDecode(dr.Field<string>("Grade")),
                                                                               }).ToList().DistinctBy(x => x.Id).OrderBy(y => y.Name).ToList();

                                        for (int k = 0; k < _Result.City[i].School[j].GradeWise.Count; k++)
                                        {

                                            _Result.City[i].School[j].GradeWise[k].Activities = (from dr in _DataSet.Tables[3].AsEnumerable()
                                                                                                 where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                                                 && dr.Field<int>("GradeId") == _Result.City[i].School[j].GradeWise[k].Id
                                                                                                 select new PieChart
                                                                                                 {
                                                                                                     Name = dr.Field<string>("Activity"),
                                                                                                     Count = dr.Field<int>("TimeSpent"),
                                                                                                 }).OrderBy(y => y.Name).ToList();
                                            val = _DataSet.Tables[3].Compute("SUM(TimeSpent)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and GradeId=" + _Result.City[i].School[j].GradeWise[k].Id) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[3].Compute("SUM(TimeSpent)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and GradeId=" + _Result.City[i].School[j].GradeWise[k].Id)) : 0;
                                            _Result.City[i].School[j].GradeWise[k].TimeSpent = val;
                                            val = _DataSet.Tables[5].Compute("SUM(TotalRead)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and GradeId=" + _Result.City[i].School[j].GradeWise[k].Id) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[5].Compute("SUM(TotalRead)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and GradeId=" + _Result.City[i].School[j].GradeWise[k].Id)) : 0;
                                            _Result.City[i].School[j].GradeWise[k].AvgBookRead = val;
                                        }



                                        _Result.City[i].School[j].OverAllActivitiesSubsection = (from dr in _DataSet.Tables[6].AsEnumerable()
                                                                                                 where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                                                 select new PieChart
                                                                                                 {
                                                                                                     Count = dr.Field<int>("Total"),
                                                                                                     Name = dr.Field<string>("Name"),
                                                                                                 }).OrderBy(y => y.Name).ToList();

                                        _Result.City[i].School[j].SubSectionWise = (from dr in _DataSet.Tables[6].AsEnumerable()
                                                                                    where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                                    select new Report2GradeSection
                                                                                    {
                                                                                        Id = dr.Field<int>("SubSectionId"),
                                                                                        Name = dr.Field<string>("Name"),
                                                                                    }).OrderBy(y => y.Name).ToList();

                                        for (int k = 0; k < _Result.City[i].School[j].SubSectionWise.Count; k++)
                                        {
                                            _Result.City[i].School[j].SubSectionWise[k].Activities = (from dr in _DataSet.Tables[8].AsEnumerable()
                                                                                                      where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId
                                                                                                      && dr.Field<int>("SubSectionId") == _Result.City[i].School[j].SubSectionWise[k].Id
                                                                                                      select new PieChart
                                                                                                      {
                                                                                                          Name = dr.Field<string>("Activity"),
                                                                                                          Count = dr.Field<int>("TimeSpent"),
                                                                                                      }).OrderBy(y => y.Name).ToList();
                                            val = _DataSet.Tables[8].Compute("SUM(TimeSpent)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and SubSectionId=" + _Result.City[i].School[j].SubSectionWise[k].Id) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[8].Compute("SUM(TimeSpent)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and SubSectionId=" + _Result.City[i].School[j].SubSectionWise[k].Id)) : 0;
                                            _Result.City[i].School[j].SubSectionWise[k].TimeSpent = val;
                                            val = _DataSet.Tables[7].Compute("SUM(TotalRead)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and SubSectionId=" + _Result.City[i].School[j].SubSectionWise[k].Id) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[7].Compute("SUM(TotalRead)", "SchoolId=" + _Result.City[i].School[j].SchoolId + " and SubSectionId=" + _Result.City[i].School[j].SubSectionWise[k].Id)) : 0;
                                            _Result.City[i].School[j].SubSectionWise[k].AvgBookRead = val;
                                        }

                                        //Student wise
                                        _Result.City[i].School[j].StudentWise.Students = (from dr in _DataSet.Tables[0].AsEnumerable()
                                                                                          where dr.Field<int>("SchoolId") == _Result.City[i].School[j].SchoolId && dr.Field<int>("GradeId") == _Result.City[i].School[j].Grade[0].Id && dr.Field<string>("Section").ToLower() == _Result.City[i].School[j].Section[0].ToLower()
                                                                                          select new StudentModel
                                                                                          {
                                                                                              FirstName = HttpUtility.HtmlDecode(dr.Field<string>("FirstName")),
                                                                                              LastName = HttpUtility.HtmlDecode(dr.Field<string>("LastName")),
                                                                                              UserId = dr.Field<int>("UserId"),
                                                                                          }).OrderBy(y => y.FirstName).ToList();
                                        //_Result.City[i].School[j].StudentWise.StudentReport
                                        Report8 _Report8 = new Report8();
                                        if (_Result.City[i].School[j].StudentWise.Students.Count() > 0)
                                        {
                                            _Report8.UserId = _Result.City[i].School[j].StudentWise.Students[0].UserId;
                                            _Report8.FirstName = HttpUtility.HtmlDecode(_Result.City[i].School[j].StudentWise.Students[0].FirstName);
                                            _Report8.LastName = HttpUtility.HtmlDecode(_Result.City[i].School[j].StudentWise.Students[0].LastName);
                                        }
                                        _Report8.Books = (from dr in _DataSet.Tables[11].AsEnumerable()
                                                          where dr.Field<int>("UserId") == _Report8.UserId
                                                          select new Report8Book
                                                          {
                                                              BookId = dr.Field<int>("BookId"),
                                                              Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                                              IsActivityDone = dr.Field<int>("IsActivityDone") == 1 ? true : false,
                                                              IsRated = dr.Field<int>("IsRated") == 1 ? true : false,
                                                              TimeSpent = dr.IsNull("TimeSpent") ? 0 : dr.Field<int>("TimeSpent"),
                                                          }).ToList();

                                        _Report8.Activities = (from dr in _DataSet.Tables[12].AsEnumerable()
                                                               where dr.Field<int>("UserId") == _Report8.UserId
                                                               select new PieChart
                                                               {
                                                                   Name = dr.Field<string>("Activity"),
                                                                   Count = dr.Field<int>("Total"),
                                                               }).OrderBy(y => y.Name).ToList();

                                        _Report8.TotalBookRated = _DataSet.Tables[11].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsRated=1") != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[11].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsRated=1")) : 0;
                                        _Report8.TotalActivityCompleted = _DataSet.Tables[11].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsActivityDone=1") != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[11].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsActivityDone=1")) : 0;
                                        _Report8.TimeSpent = _DataSet.Tables[12].Compute("Sum(Total)", "UserId=" + _Report8.UserId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[12].Compute("Sum(Total)", "UserId=" + _Report8.UserId)) : 0;
                                        _Result.City[i].School[j].StudentWise.StudentReport.Add(_Report8);
                                    }
                                }
                            }
                            else
                            {
                                // _Result.Status = Report6Status.Error;
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport2", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, GradeIds={2}, City={3}", StartDate, EndDate, GradeIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport2", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, GradeIds={2}, City={3}", StartDate, EndDate, GradeIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public Report2StudentWise GetReport2StudentWise(DateTime StartDate, DateTime EndDate, string Section, int UserId, int GradeId, int SchoolId,string CallFrom )
        {
            Report2StudentWise _Result = new Report2StudentWise();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport2StudentWise", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@Section", SqlDbType.NVarChar).Value = Section;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;
                        command.Parameters.Add("@GradeId", SqlDbType.Int).Value = GradeId;
                        command.Parameters.Add("@callfrom", SqlDbType.VarChar).Value = CallFrom;
                        

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow item in _DataSet.Tables[0].Rows)
                                {
                                    _Result.Students.Add(new StudentModel() { UserId = Convert.ToInt32(item["UserId"]), FirstName = HttpUtility.HtmlDecode(item["FirstName"].ToString()), LastName = HttpUtility.HtmlDecode(item["LastName"].ToString()) });
                                }

                                Report8 _Report8 = new Report8();
                                if (_Result.Students.Count > 0)
                                {
                                    if (UserId == 0)
                                    {
                                        _Report8.UserId = _Result.Students[0].UserId;
                                        _Report8.FirstName = HttpUtility.HtmlDecode(_Result.Students[0].FirstName);
                                        _Report8.LastName = HttpUtility.HtmlDecode(_Result.Students[0].LastName);
                                    }
                                    else
                                    {
                                        dynamic mod = "";
                                        mod = (from item in _Result.Students where item.UserId == UserId select item).FirstOrDefault();
                                        _Report8.UserId = mod.UserId;
                                        _Report8.FirstName = HttpUtility.HtmlDecode(mod.FirstName);
                                        _Report8.LastName = HttpUtility.HtmlDecode(mod.LastName);
                                    }
                                    _Report8.Books = (from dr in _DataSet.Tables[1].AsEnumerable()
                                                      where dr.Field<int>("UserId") == _Report8.UserId
                                                      select new Report8Book
                                                      {
                                                          BookId = dr.Field<int>("BookId"),
                                                          Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                                          IsActivityDone = dr.Field<int>("IsActivityDone") == 1 ? true : false,
                                                          IsRated = dr.Field<int>("IsRated") == 1 ? true : false,
                                                          TimeSpent = dr.IsNull("TimeSpent") ? 0 : dr.Field<int>("TimeSpent"),
                                                      }).ToList();

                                    _Report8.Activities = (from dr in _DataSet.Tables[2].AsEnumerable()
                                                           where dr.Field<int>("UserId") == _Report8.UserId
                                                           select new PieChart
                                                           {
                                                               Name = dr.Field<string>("Activity"),
                                                               Count = dr.Field<int>("Total"),
                                                           }).ToList();
                                    _Report8.TotalBookRated = _DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsRated=1") != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsRated=1")) : 0;
                                    _Report8.TotalActivityCompleted = _DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsActivityDone=1") != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[1].Compute("Count(BookId)", "UserId=" + _Report8.UserId + " And IsActivityDone=1")) : 0;
                                    _Report8.TimeSpent = _DataSet.Tables[2].Compute("Sum(Total)", "UserId=" + _Report8.UserId) != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[2].Compute("Sum(Total)", "UserId=" + _Report8.UserId)) : 0;
                                    _Result.StudentReport.Add(_Report8);
                                }

                                if (!string.IsNullOrEmpty(CallFrom) && CallFrom.ToLower() == "grade")
                                {
                                    foreach (DataRow row in _DataSet.Tables[3].Rows)
                                    {
                                        _Result.Section.Add(row["Section"].ToString());
                                    }
                                }

                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport2StudentWise", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, Section={2}, GradeId={3}, UserId={4}", StartDate, EndDate, Section, GradeId, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport2StudentWise", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, Section={2}, GradeId={3}, UserId={4}", StartDate, EndDate, Section, GradeId, UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public List<Report2Export> GetReport2Export(DateTime StartDate, DateTime EndDate, string SchoolIds, string GradeIds, string City)
        {
            List<Report2Export> _Result = new List<Report2Export>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport2Export", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolIds", SqlDbType.VarChar).Value = string.IsNullOrEmpty(SchoolIds) ? "" : SchoolIds; ;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@GradeIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(GradeIds) ? "" : GradeIds;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result = (from item in _DataSet.Tables[0].AsEnumerable()
                                           select new Report2Export
                                           {
                                               SchoolName = HttpUtility.HtmlDecode(item.Field<string>("SchoolName")),
                                               FirstName = HttpUtility.HtmlDecode(item.Field<string>("FirstName")),
                                               LastName = HttpUtility.HtmlDecode(item.Field<string>("LastName")),
                                               Section = HttpUtility.HtmlDecode(item.Field<string>("Section")),
                                               SubSection = item.Field<string>("SubSection"),
                                               Grade = item.Field<string>("Grade"),
                                               City = item.Field<string>("City"),
                                               TimeSpentActivity = (float)Math.Round((double)item.Field<int>("TimeSpentActivity") / 60, 1),
                                               TimeSpentBookRead = (float)Math.Round((double)item.Field<int>("TimeSpentBookRead") / 60, 1),
                                               TimeSpentBrowsing = (float)Math.Round((double)item.Field<int>("TimeSpentBrowsing") / 60, 1),
                                               TotalActivitiesCompleted = item.IsNull("TotalActivitiesCompleted") ? 0 : item.Field<int>("TotalActivitiesCompleted"),
                                               TotalBookRead = item.IsNull("TotalBookRead") ? 0 : item.Field<int>("TotalBookRead"),
                                           }).ToList();
                            }
                            else
                            {
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport2Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, GradeIds={2}, City={3}", StartDate, EndDate, GradeIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport2Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, GradeIds={2}, City={3}", StartDate, EndDate, GradeIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public Report5FilterModel GetReport5Fillters(int UserId)
        {

            Report5FilterModel _Result = new Report5FilterModel();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport5Fillters", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 2)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    _Result.Cities.Add(_DataRow["City"].ToString());
                                }

                                foreach (DataRow dr in _DataSet.Tables[1].Rows)
                                {
                                    _Result.Schools.Add(new School() { SchoolId = Convert.ToInt32(dr["SchoolId"]), SchoolName = HttpUtility.HtmlDecode(dr["SchoolName"].ToString()), City = dr["City"].ToString() });
                                }

                                foreach (DataRow dr in _DataSet.Tables[2].Rows)
                                {
                                    _Result.Grades.Add(new Grade() { Id = Convert.ToInt32(dr["Id"]), Name = dr["Name"].ToString() });
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport5Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport5Fillters", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report5 GetReport5(DateTime StartDate, DateTime EndDate, int SchoolId, string GradeIds)
        {
            Report5 _Result = new Report5();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport5", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@GradeIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(GradeIds) ? "" : GradeIds;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow item in _DataSet.Tables[0].Rows)
                                {
                                    _Result.BookRead.Add(new Report5Data()
                                    {
                                        GradeId = Convert.ToInt32(item["GradeId"]),
                                        GradeName = item["GradeName"].ToString(),
                                        OverAll = Convert.ToSingle(item["OverAll"]),
                                        School = Convert.ToSingle(item["School"])
                                    });
                                }

                                foreach (DataRow item in _DataSet.Tables[1].Rows)
                                {
                                    _Result.TimeSpent.Add(new Report5Data()
                                    {
                                        GradeId = Convert.ToInt32(item["GradeId"]),
                                        GradeName = item["GradeName"].ToString(),
                                        OverAll = Convert.ToSingle(item["OverAll"]),
                                        School = Convert.ToSingle(item["School"])
                                    });
                                }
                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport5", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, GradeIds={2}, SchoolId={3}", StartDate, EndDate, GradeIds, SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport5", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, GradeIds={2}, SchoolId={3}", StartDate, EndDate, GradeIds, SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public List<Report5Export> GetReport5Export(DateTime StartDate, DateTime EndDate, int SchoolId, string GradeIds)
        {
            List<Report5Export> _Result = new List<Report5Export>();
            _Result.Add(new Report5Export());
            _Result.Add(new Report5Export());
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport5", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SchoolId", SqlDbType.Int).Value = SchoolId;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@GradeIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(GradeIds) ? "" : GradeIds;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result[0].OverAll = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("GradeId") == 0).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].School = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("GradeId") == 0).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade1 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 1).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade1 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 1).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade2 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 2).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade2 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 2).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade3 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 3).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade3 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 3).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade4 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 4).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade4 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 4).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade5 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 5).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade5 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 5).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade6 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 6).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade6 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 6).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade7 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 7).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade7 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 7).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[0].OverAllGrade8 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 8).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[0].SchoolGrade8 = _DataSet.Tables[0].AsEnumerable().Where(x => x.Field<int>("Grade") == 8).Select(y => y.Field<string>("School")).FirstOrDefault();

                                _Result[1].OverAll = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("GradeId") == 0).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].School = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("GradeId") == 0).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade1 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 1).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade1 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 1).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade2 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 2).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade2 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 2).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade3 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 3).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade3 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 3).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade4 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 4).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade4 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 4).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade5 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 5).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade5 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 5).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade6 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 6).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade6 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 6).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade7 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 7).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade7 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 7).Select(y => y.Field<string>("School")).FirstOrDefault();
                                _Result[1].OverAllGrade8 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 8).Select(y => y.Field<string>("OverAll")).FirstOrDefault();
                                _Result[1].SchoolGrade8 = _DataSet.Tables[1].AsEnumerable().Where(x => x.Field<int>("Grade") == 8).Select(y => y.Field<string>("School")).FirstOrDefault();

                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport5Export", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, GradeIds={2}, SchoolId={3}", StartDate, EndDate, GradeIds, SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport5Export", string.Format("Error occured while getting report  StartDate={0}, EndDate={1}, GradeIds={2}, SchoolId={3}", StartDate, EndDate, GradeIds, SchoolId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public List<Report3Export> GetReport3Export(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string BookIds, string City)
        {
            List<Report3Export> _Result = new List<Report3Export>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport3Export", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LanguageIds", SqlDbType.VarChar).Value = LanguageIds;
                        command.Parameters.Add("@SubsectionIds", SqlDbType.VarChar).Value = SubsectionIds;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@BookIds", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(BookIds) ? "" : BookIds;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result = (from dr in _DataSet.Tables[0].AsEnumerable()
                                           select new Report3Export
                                           {
                                               BookId = dr.Field<int>("BookId"),
                                               Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                               Language = dr.Field<string>("Language"),
                                               SubSection = dr.Field<string>("SubSection")
                                           }).ToList();


                                for (int i = 0; i < _Result.Count(); i++)
                                {
                                    _Result[i].Grade1Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "1").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade2Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "2").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade3Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "3").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade4Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "4").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade5Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "5").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade6Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "6").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade7Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "7").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade8Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "8").Select(a => a.Field<int>("Count")).FirstOrDefault();

                                    _Result[i].Grade1Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "1").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade2Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "2").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade3Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "3").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade4Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "4").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade5Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "5").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade6Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "6").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade7Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "7").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade8Activity = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "8").Select(a => a.Field<int>("Count")).FirstOrDefault();

                                    _Result[i].Rating1 = _DataSet.Tables[3].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "1").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating2 = _DataSet.Tables[3].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "2").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating3 = _DataSet.Tables[3].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "3").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating4 = _DataSet.Tables[3].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "4").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating5 = _DataSet.Tables[3].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "5").Select(a => a.Field<int>("Count")).FirstOrDefault();

                                    _Result[i].TotalRead = _DataSet.Tables[4].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId).Select(a => a.Field<int>("TotalRead")).FirstOrDefault();
                                    _Result[i].TotalReadCompleted = _DataSet.Tables[4].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId).Select(a => a.Field<int>("TotalCompleted")).FirstOrDefault();
                                }

                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport3Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, LanguageIds={2}, SubsectionIds={3}, City={4}", StartDate, EndDate, LanguageIds, SubsectionIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport3Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, LanguageIds={2}, SubsectionIds={3}, City={4}", StartDate, EndDate, LanguageIds, SubsectionIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public List<Report3Export> GetReport4Export(DateTime StartDate, DateTime EndDate, string LanguageIds, string SubsectionIds, string Publisher, string City)
        {
            List<Report3Export> _Result = new List<Report3Export>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetReport4Export", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LanguageIds", SqlDbType.VarChar).Value = LanguageIds;
                        command.Parameters.Add("@SubsectionIds", SqlDbType.VarChar).Value = SubsectionIds;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@Publisher", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(Publisher) ? "" : Publisher;
                        command.Parameters.Add("@City", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(City) ? "" : City;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result = (from dr in _DataSet.Tables[0].AsEnumerable()
                                           select new Report3Export
                                           {
                                               BookId = dr.Field<int>("BookId"),
                                               Title = HttpUtility.HtmlDecode(dr.Field<string>("Title")),
                                               Language = dr.Field<string>("Language"),
                                               SubSection = dr.Field<string>("SubSection"),
                                               Publisher = HttpUtility.HtmlDecode(dr.Field<string>("Publisher"))
                                           }).ToList();


                                for (int i = 0; i < _Result.Count(); i++)
                                {
                                    _Result[i].Grade1Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "1").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade2Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "2").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade3Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "3").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade4Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "4").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade5Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "5").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade6Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "6").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade7Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "7").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Grade8Read = _DataSet.Tables[1].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("GradeName") == "8").Select(a => a.Field<int>("Count")).FirstOrDefault();

                                    _Result[i].Rating1 = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "1").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating2 = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "2").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating3 = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "3").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating4 = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "4").Select(a => a.Field<int>("Count")).FirstOrDefault();
                                    _Result[i].Rating5 = _DataSet.Tables[2].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId && s.Field<string>("Name") == "5").Select(a => a.Field<int>("Count")).FirstOrDefault();

                                    _Result[i].TotalRead = _DataSet.Tables[3].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId).Select(a => a.Field<int>("TotalRead")).FirstOrDefault();
                                    _Result[i].TotalReadCompleted = _DataSet.Tables[3].AsEnumerable().Where(s => s.Field<int>("BookId") == _Result[i].BookId).Select(a => a.Field<int>("TotalCompleted")).FirstOrDefault();
                                }

                            }

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport4Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, LanguageIds={2}, SubsectionIds={3}, City={4}", StartDate, EndDate, LanguageIds, SubsectionIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport4Export", string.Format("Error occured while exporting report  StartDate={0}, EndDate={1}, LanguageIds={2}, SubsectionIds={3}, City={4}", StartDate, EndDate, LanguageIds, SubsectionIds, City), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public PagedList<StudentModel> GetReport1SchoolAdminList(string SchoolUId, int PageSize, int PageIndex, string Type, int GradeId, string Section)
        {
            PagedList<StudentModel> _Result = new PagedList<StudentModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spReport1SchoolAdminList", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@GradeId", SqlDbType.Int).Value = GradeId;
                        command.Parameters.Add("@Section", SqlDbType.VarChar).Value = Section;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);


                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    StudentModel _Student = new StudentModel();
                                    _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Student.Email = _DataRow["Email"].ToString();
                                    _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _Student.Username = _DataRow["Username"].ToString();
                                    _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _Student.Gender = _DataRow["Gender"].ToString();
                                    _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                    _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                    _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                    if (Type == "created")
                                        _Student.Result = "Created but Not Registered";
                                    else if (Type == "registered")
                                        _Student.Result = "Registered but Never Logged";
                                    else
                                        _Student.Result = "Active";
                                    _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                    _Student.Grade = _DataRow["Grade"].ToString();
                                    _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Items.Add(_Student);
                                }
                                _Result.TotalItems = _DataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["TotalRows"]) : 0;
                                _Result.PageSize = PageSize;
                                _Result.PageIndex = PageIndex;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport1SchoolAdminList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport1SchoolAdminList", ex.Message.ToString(), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public Report1SchoolAdmin GetReport1SchoolAdmin(string SchoolUId, int PageSize, int PageIndex, bool IsExport = false)
        {
            Report1SchoolAdmin _Result = new Report1SchoolAdmin();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spReport1SchoolAdmin", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                        command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                        command.Parameters.Add("@SchoolUId", SqlDbType.VarChar).Value = SchoolUId;
                        command.Parameters.Add("@IsExport", SqlDbType.Bit).Value = IsExport;
                        
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 4)
                            {

                                _Result.Grade = (from item in _DataSet.Tables[0].AsEnumerable()
                                                 select new Grade
                                                 {
                                                     Id = item.Field<int>("Id"),
                                                     Name = item.Field<string>("Name")
                                                 }).ToList();

                                foreach (DataRow item in _DataSet.Tables[1].Rows)
                                {
                                    _Result.Section.Add(item["Section"].ToString());
                                }

                                foreach (DataRow _DataRow in _DataSet.Tables[2].Rows)
                                {
                                    StudentModel _Student = new StudentModel();
                                    _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Student.Email = _DataRow["Email"].ToString();
                                    _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _Student.Username = _DataRow["Username"].ToString();
                                    _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _Student.Gender = _DataRow["Gender"].ToString();
                                    _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                    _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                    _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                    _Student.Result = "Created but Not Registered";
                                    _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                    _Student.Grade = _DataRow["Grade"].ToString();
                                    _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Created.Items.Add(_Student);
                                }
                                _Result.Created.TotalItems = _DataSet.Tables[2].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[2].Rows[0]["TotalRows"]) : 0;
                                _Result.Created.PageSize = PageSize;
                                _Result.Created.PageIndex = PageIndex;
                                _Result.CreatedTotal = _DataSet.Tables[3].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[3].Rows[0]["AllTotalRows"]) : 0;
                                foreach (DataRow _DataRow in _DataSet.Tables[4].Rows)
                                {
                                    StudentModel _Student = new StudentModel();
                                    _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Student.Email = _DataRow["Email"].ToString();
                                    _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _Student.Username = _DataRow["Username"].ToString();
                                    _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _Student.Gender = _DataRow["Gender"].ToString();
                                    _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                    _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                    _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                    _Student.Result = "Registered but Never Logged";
                                    _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                    _Student.Grade = _DataRow["Grade"].ToString();
                                    _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Registered.Items.Add(_Student);
                                }
                                _Result.Registered.TotalItems = _DataSet.Tables[4].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[4].Rows[0]["TotalRows"]) : 0;
                                _Result.Registered.PageSize = PageSize;
                                _Result.Registered.PageIndex = PageIndex;
                                _Result.RegisteredTotal = _DataSet.Tables[5].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[5].Rows[0]["AllTotalRows"]) : 0;

                                foreach (DataRow _DataRow in _DataSet.Tables[6].Rows)
                                {
                                    StudentModel _Student = new StudentModel();
                                    _Student.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _Student.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _Student.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _Student.Email = _DataRow["Email"].ToString();
                                    _Student.MobileNumber = _DataRow["MobileNo"].ToString();
                                    _Student.Username = _DataRow["Username"].ToString();
                                    _Student.CreationDate = DateTime.Parse(_DataRow["CreationDate"].ToString());
                                    _Student.Gender = _DataRow["Gender"].ToString();
                                    _Student.RegistrationDate = _DataRow["RegistrationDate"] != DBNull.Value ? DateTime.Parse(_DataRow["RegistrationDate"].ToString()) : (DateTime?)null;
                                    _Student.Status = bool.Parse(_DataRow["Status"].ToString());
                                    _Student.IsTrashed = bool.Parse(_DataRow["IsTrashed"].ToString());
                                    _Student.SubscriptionStartDate = _DataRow["SubscriptionStartDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionStartDate"].ToString()) : DateTime.Now;
                                    _Student.SubscriptionEndDate = _DataRow["SubscriptionEndDate"] != DBNull.Value ? DateTime.Parse(_DataRow["SubscriptionEndDate"].ToString()) : DateTime.Now;
                                    _Student.Result = "Active";
                                    _Student.SchoolName = HttpUtility.HtmlDecode(_DataRow["SchoolName"].ToString());
                                    _Student.Grade = _DataRow["Grade"].ToString();
                                    _Student.LastLoginDate = _DataRow["LastLoginDate"] != DBNull.Value ? DateTime.Parse(_DataRow["LastLoginDate"].ToString()) : (DateTime?)null;
                                    _Result.Active.Items.Add(_Student);
                                }
                                _Result.Active.TotalItems = _DataSet.Tables[6].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[6].Rows[0]["TotalRows"]) : 0;
                                _Result.Active.PageSize = PageSize;
                                _Result.Active.PageIndex = PageIndex;
                                _Result.ActiveTotal = _DataSet.Tables[7].Rows.Count > 0 ? Convert.ToInt32(_DataSet.Tables[7].Rows[0]["AllTotalRows"]) : 0;

                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetReport1SchoolAdmin", string.Format("Error occured while exporting report  SchoolUId={0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetReport1SchoolAdmin", string.Format("Error occured while exporting report  SchoolUId={0}", SchoolUId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result = null;
            }
            return _Result;
        }

        public string GetUsernameByToken(string token)
        {
            Report5 _Result = new Report5();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUsernameByToken", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Token", SqlDbType.VarChar).Value = token;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                                return _DataSet.Tables[0].Rows[0]["Username"].ToString();
                            else
                                return null;

                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public bool SyncUserData(UserProgressForSync UserSyncData, string Type, int DataId)
        {
            try
            {
                if (UserSyncData.UserId > 0)
                    _currentUserId = UserSyncData.UserId;

                double Browsingtime = 0;
                var booksxml = "";
                if (Type != "BookData")
                {
                    if (UserSyncData.BrowsingProgress != null && UserSyncData.BrowsingProgress.Progress.Count > 0)
                    {
                        foreach (Progress bdata in UserSyncData.BrowsingProgress.Progress)
                        {
                            Browsingtime += Math.Round(bdata.EndTime.Subtract(bdata.StartTime).TotalSeconds);
                        }
                        if (Browsingtime > 0)
                            Browsingtime = Math.Round(Browsingtime / 60);//CALCULATE TOTAL MINS
                    }
                }
                if (Type != "BrowsingData")
                {
                    if (UserSyncData.UserProgressBooks != null && UserSyncData.UserProgressBooks.UserProgressBook.Count > 0)
                    {
                        var books = XDocument.Parse("<Books></Books>");
                        foreach (UserProgressBook bdata in UserSyncData.UserProgressBooks.UserProgressBook)
                        {
                            double ActivityTime = 0;
                            double ReadingTime = 0;
                            double ReviewTime = 0;
                            if (bdata.Activity.ActivityProgress != null && bdata.Activity.ActivityProgress.Count > 0)
                            {
                                foreach (ActivityProgress adata in bdata.Activity.ActivityProgress)
                                {
                                    ActivityTime += Math.Round(adata.EndTime.Subtract(adata.StartTime).TotalSeconds);
                                }
                                if (ActivityTime > 0)
                                    ActivityTime = Math.Round(ActivityTime / 60);//CALCULATE TOTAL MINS
                            }

                            if (bdata.ReadingProgress != null && bdata.ReadingProgress.Pages.Page != null && bdata.ReadingProgress.Pages.Page.Count > 0)
                            {
                                // to have distict pages based on Start time and Endtime because if user is reading in double pager mode than start time and end time will be same for both pages.
                                //double sum = bdata.ReadingProgress.Pages.Page.Sum(x => x.EndTime.Subtract(x.StartTime).TotalSeconds);
                                List<Page> pages= bdata.ReadingProgress.Pages.Page.GroupBy(c => new { c.StartTime, c.EndTime}).Select(c => c.First()).ToList();                                
                                foreach (Page rdata in pages)
                                {
                                    ReadingTime += Math.Round(rdata.EndTime.Subtract(rdata.StartTime).TotalSeconds);
                                }
                                if (ReadingTime > 0)
                                    ReadingTime = Math.Round(ReadingTime / 60);//CALCULATE TOTAL MINS
                            }
                            if (bdata.BookReview.ReviewProgress != null && bdata.BookReview.ReviewProgress.Count > 0)
                            {
                                foreach (ReviewProgress rdata in bdata.BookReview.ReviewProgress)
                                {
                                    ReviewTime += Math.Round(rdata.EndTime.Subtract(rdata.StartTime).TotalSeconds);
                                }
                                if (ReviewTime > 0)
                                    ReviewTime = Math.Round(ReviewTime / 60);//CALCULATE TOTAL MINS
                            }

                            var Book = new XElement("Book",
                               new XElement("BookId", bdata.BookId),
                               new XElement("IsRead", bdata.IsRead),
                               new XElement("IsReadLater", bdata.IsReadLater),
                               new XElement("ReadLaterOn", bdata.ReadLaterOn),
                               new XElement("LastDateAccessed", bdata.LastDateAccessed),
                               new XElement("BookCompletedOn", bdata.BookCompletedOn),
                               new XElement("CurrentPage", bdata.CurrentPage),
                               new XElement("IsActivityDone", bdata.Activity.IsActivityDone),
                               new XElement("ActivityCompletedOn", bdata.Activity.ActivityCompletedOn),
                               new XElement("ActivityJson", bdata.Activity.ActivityJson),
                               new XElement("ActivityTime", ActivityTime),
                               new XElement("Rating", (bdata.RatingLog.Log != null && bdata.RatingLog.Log.Count > 0) ? bdata.RatingLog.Log.OrderByDescending(x => x.RatedOn).FirstOrDefault().Rating : 0),
                               new XElement("RatedOn", (bdata.RatingLog.Log != null && bdata.RatingLog.Log.Count > 0) ? bdata.RatingLog.Log.OrderByDescending(x => x.RatedOn).FirstOrDefault().RatedOn : null),
                               new XElement("ReadingTime", ReadingTime),
                               new XElement("BookReadStartedOn", (bdata.ReadingProgress != null && bdata.ReadingProgress.Pages.Page != null && bdata.ReadingProgress.Pages.Page.Count > 0) ? bdata.ReadingProgress.Pages.Page.OrderBy(x => x.StartTime).FirstOrDefault().StartTime.ToString() : null),
                               new XElement("IsReviewDone", bdata.BookReview.IsReviewDone),
                               new XElement("ReviewCompletedOn", bdata.BookReview.ReviewCompletedOn),
                               new XElement("ReviewJson", bdata.BookReview.ReviewJson),
                               new XElement("ReviewTime", ReviewTime)
                               );
                            books.Root.Add(Book);
                        }
                        booksxml = books.ToString();
                    }
                }
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSyncUserData", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserSyncData.UserId;
                        command.Parameters.Add("@DataId", SqlDbType.Int).Value = DataId;
                        command.Parameters.Add("@BrowsingTime", SqlDbType.Int).Value = Browsingtime;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                        command.Parameters.Add("@XML1", SqlDbType.NVarChar).Value = booksxml;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        int _Status = (int)Status.Value;
                        if (_Status == 1)
                        {
                            InsertLog(_currentUserId, "SyncUserData", "Sync User Data", string.Format("Synching DataId = {0} of type = {1} for user {2}", DataId, Type, _currentUserId), UserStatus.Sucess.ToString());
                            return true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                InsertLog(_currentUserId, "SyncUserData", string.Format("Error occured while synching DataId = {0} of type = {1} for user {2}", DataId, Type, _currentUserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                return false;
            }
            return false;
        }
        public int InsertSyncUserData(int UserId, string UserSyncData, string Type)
        {
            int _Status = 0;
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spInsertUserSyncData", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@Data", SqlDbType.NVarChar).Value = UserSyncData;
                        command.Parameters.Add("@SyncType", SqlDbType.VarChar).Value = Type;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;
                        InsertLog(_currentUserId, "InsertSyncUserData", "Insert Sync UserData", string.Format("Inserting Sync data of type = {0} for user {1}", Type, _currentUserId), UserStatus.Sucess.ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                _Status = 0;
                InsertLog(_currentUserId, "InsertSyncUserData", string.Format("Error occured while inserting Sync data of type = {0} for user {1}", Type, _currentUserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Status;
        }
        public UserDetails GetUserBooksDetails(int UserId)
        {
            UserDetails _Result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUserBookDataDetails", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result = new UserDetails();
                                if (_DataSet.Tables[0].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        _Result.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Result.TotalBookRead = int.Parse(_DataRow["TotalBookRead"].ToString());
                                        _Result.TotalBookRated = int.Parse(_DataRow["TotalBookRated"].ToString());
                                        _Result.TotalActivitiesCompleted = int.Parse(_DataRow["TotalActivitiesCompleted"].ToString());
                                        _Result.TotalHourSpent = double.Parse(_DataRow["TotalHourSpent"].ToString());
                                        _Result.LastAccessedBookId = int.Parse(_DataRow["LastAccessedBookId"].ToString());
                                        _Result.LastReadLaterBookId = int.Parse(_DataRow["LastReadLaterBookId"].ToString());
                                        _Result.TotalHourSpentOnActivity = int.Parse(_DataRow["TotalHourSpentOnActivity"].ToString());
                                        _Result.TotalHourSpentOnReading = int.Parse(_DataRow["TotalHourSpentOnReading"].ToString());
                                        _Result.TotalHourSpentOnReview = int.Parse(_DataRow["TotalHourSpentOnReview"].ToString());
                                    }
                                }

                                if (_DataSet.Tables[1].Rows.Count != 0)
                                {

                                    foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                    {
                                        DeviceBook _books = new DeviceBook();
                                        _books.Rating = _DataRow["Rating"] != DBNull.Value ? Convert.ToInt32(_DataRow["Rating"]) : 0;
                                        _books.BookId = _DataRow["BookId"] != DBNull.Value ? Convert.ToInt32(_DataRow["BookId"]) : 0;
                                        _books.IsReadLater = _DataRow["IsReadLater"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsReadLater"]) : false;
                                        _books.IsRead = _DataRow["IsRead"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsRead"]) : false;
                                        _books.IsActivityDone = _DataRow["IsActivityDone"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsActivityDone"]) : false;
                                        _books.ActivityJson = _DataRow["ActivityJson"] != DBNull.Value ? Regex.Unescape(_DataRow["ActivityJson"].ToString()) : "";
                                        _books.IsReviewDone = _DataRow["IsReviewDone"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsReviewDone"]) : false;
                                        _books.ReviewJson = _DataRow["ReviewJson"] != DBNull.Value ? Regex.Unescape(_DataRow["ReviewJson"].ToString()) : "";
                                        _books.LastDateAccessed = _DataRow["LastDateAccessed"] != DBNull.Value ? Convert.ToDateTime(_DataRow["LastDateAccessed"]) : (DateTime?)null;
                                        _books.ReviewCompletedOn = _DataRow["ReviewCompletedOn"] != DBNull.Value ? Convert.ToDateTime(_DataRow["ReviewCompletedOn"]) : (DateTime?)null;
                                        _books.ActivityCompletedOn = _DataRow["ActivityCompletedOn"] != DBNull.Value ? Convert.ToDateTime(_DataRow["ActivityCompletedOn"]) : (DateTime?)null;
                                        _books.BookCompletedOn = _DataRow["BookCompletedOn"] != DBNull.Value ? Convert.ToDateTime(_DataRow["BookCompletedOn"]) : (DateTime?)null;
                                        _books.Bookmark.CurrentPage = _DataRow["CurrentPage"] != DBNull.Value ? Convert.ToInt32(_DataRow["CurrentPage"]) : 0;
                                        _books.Bookmark.ReadingTime = _DataRow["BookCompletionTime"] != DBNull.Value ? Convert.ToInt32(_DataRow["BookCompletionTime"]) : 0;
                                        _books.Bookmark.ReviewTime = _DataRow["ReviewCompletionTime"] != DBNull.Value ? Convert.ToInt32(_DataRow["ReviewCompletionTime"]) : 0;
                                        _books.Bookmark.ActivityTime = _DataRow["ActivityCompletionTime"] != DBNull.Value ? Convert.ToInt32(_DataRow["ActivityCompletionTime"]) : 0;

                                        _books.BookReadStartedOn = _DataRow["BookReadStartedOn"] != DBNull.Value ? Convert.ToDateTime(_DataRow["BookReadStartedOn"]) : (DateTime?)null;
                                        _books.RatedOn = _DataRow["RatedOn"] != DBNull.Value ? Convert.ToDateTime(_DataRow["RatedOn"]) : (DateTime?)null;
                                        _books.ReadLaterOn = _DataRow["ReadLaterOn"] != DBNull.Value ? Convert.ToDateTime(_DataRow["ReadLaterOn"]) : (DateTime?)null;

                                        foreach (string bookdevices in _DataRow["Devices"].ToString().Split(','))
                                        {
                                            if (bookdevices != "")
                                            {
                                                _books.DeviceId.Add(Convert.ToInt32(bookdevices));
                                            }
                                        }
                                        _Result.Books.Add(_books);
                                    }
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetUserBooksDetails", string.Format("Error occured whiel getting book details for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetUserBooksDetails", string.Format("Error occured whiel getting book details for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public void UpdateLastLogin(int UserId)
        {
            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateLastLogin", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        InsertLog(_currentUserId, "UserLogin", "User logged in", string.Format("User with Id {0} logged in",_currentUserId), UserStatus.Sucess.ToString());
                    }
                }
            }
            catch
            {
                InsertLog(_currentUserId, "UserLogin", "Error occured while User logged in", string.Format("Error occured while User with Id {0} logged in", _currentUserId), UserStatus.Error.ToString());
            }
        }

        public void CheckBrowserLogin(int UserId, string SessionId, string Plateform, string Browser, string IPAddress)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spCheckBrowserLogin", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@SessionId", SqlDbType.VarChar).Value = SessionId;
                        command.Parameters.Add("@Plateform", SqlDbType.VarChar).Value = Plateform;
                        command.Parameters.Add("@Browser", SqlDbType.VarChar).Value = Browser;
                        command.Parameters.Add("@IPAddress", SqlDbType.VarChar).Value = IPAddress;
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {

            }
        }

        public void InsertLog(int UserId, string Activity, string ShortMessage, string FullMessage, string Status)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spInsertLog", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (UserId != 0)
                            command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@Activity", SqlDbType.VarChar).Value = Activity;
                        command.Parameters.Add("@ShortMessage", SqlDbType.VarChar).Value = ShortMessage;
                        command.Parameters.Add("@FullMessage", SqlDbType.VarChar).Value = FullMessage;
                        command.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                        }
                        catch (SqlException ex)
                        {

                        }
                    }
                }
            }
            catch (SqlException ex)
            {

            }
        }

        public UserResult UpdateCreatedUser(string Email, string MobileNo, int UserId, string UserType)
        {
            int _Status = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateCreatedUser", con))
                    {
                        string Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                        command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = MobileNo;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@UserType", SqlDbType.NVarChar).Value = UserType;
                        SqlParameter Status = new SqlParameter("@Status", SqlDbType.Int);
                        Status.Direction = ParameterDirection.Output;
                        command.Parameters.Add(Status);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        _Status = (int)Status.Value;

                        if (_Status == 1)
                        {
                            User _User = new User();
                            _User.Token = Token;
                            InsertLog(_currentUserId, "UpdateCreatedUser", "Update user in created state", string.Format("{0} updated with Email={1}, Mobile={2}", UserType, Email, MobileNo), UserStatus.Sucess.ToString());
                            return new UserResult { User = _User, Status = UserStatus.Sucess };
                        }
                        else if (_Status == 2)
                        {
                            InsertLog(_currentUserId, "UpdateCreatedUser", "", "", UserStatus.UserAlreadyRegistered.ToString());
                            return new UserResult { Status = UserStatus.UserAlreadyRegistered };
                        }
                        else if (_Status == 3)
                        {
                            InsertLog(_currentUserId, "UpdateCreatedUser", "Update user in created state", string.Format("{0} updated with Email={1}, Mobile={2}", UserType, Email, MobileNo), UserStatus.UserAccountNotExist.ToString());
                            return new UserResult { Status = UserStatus.UserAccountNotExist };
                        }
                        else if (_Status == -1)
                        {
                            InsertLog(_currentUserId, "UpdateCreatedUser", "Update user in created state", string.Format("{0} updated with Email={1}, Mobile={2}", UserType, Email, MobileNo), UserStatus.Error.ToString());
                            return new UserResult { Status = UserStatus.Error };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "UpdateCreatedUser", "Failed to update user", ex.InnerException.ToString(), UserStatus.Error.ToString());
                return new UserResult { Status = UserStatus.Error };
            }
            return new UserResult { Status = UserStatus.Error };
        }

        public string GetLastemailSendDate(int UserId, string Type)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetLastemailSendDate", con))
                    {
                        string Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                                return _DataSet.Tables[0].Rows[0][0].ToString();
                        }
                        catch
                        {
                            return "";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return "";

            }
            return "";
        }

        public UserPasswordRecovery SendPasswordRecoveryReader(int UserId)
        {
            UserPasswordRecovery _result = new UserPasswordRecovery();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetParentAndSchoolAdmins", con))
                    {
                        string _Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);


                            if (_DataSet.Tables.Count > 0)
                            {
                                foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                {
                                    PasswordRecoveryUsers _User = new PasswordRecoveryUsers();
                                    _User.UserId = int.Parse(_DataRow["UserId"].ToString());
                                    _User.FirstName = HttpUtility.HtmlDecode(_DataRow["FirstName"].ToString());
                                    _User.LastName = HttpUtility.HtmlDecode(_DataRow["LastName"].ToString());
                                    _User.Role = _DataRow["Role"].ToString();
                                    _result.Users.Add(_User);
                                }
                                _result.Status = UserStatus.Sucess;
                            }
                        }
                        catch
                        {
                            _result.Status = UserStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _result.Status = UserStatus.Error;
            }
            return _result;
        }

        public UserPasswordRecovery SendSchoolStudentPasswordRecovery(string UserName, int ToId)
        {
            int Status = 0;
            UserPasswordRecovery _result = new UserPasswordRecovery();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spInsertSchoolStudnetPasswordRecoveryToken", con))
                    {
                        string _Token = Guid.NewGuid().ToString();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
                        command.Parameters.Add("@ToId", SqlDbType.Int).Value = ToId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        Status = (int)id.Value;
                        if (Status == 1)
                        {
                            _result.Status = UserStatus.Sucess;
                        }
                        else if (Status == 2)
                        {
                            _result.Status = UserStatus.UserAccountNotExist;
                        }
                        else
                        {
                            _result.Status = UserStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _result.Status = UserStatus.Error;
            }
            return _result;
        }

        public BookRead GetBookDetail(int userId, int bookId)
        {
            BookRead _Result = new BookRead();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBookDetail", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                        command.Parameters.Add("@bookId", SqlDbType.Int).Value = bookId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if ((int)id.Value == 1)
                            {
                                if (_DataSet.Tables.Count > 0)
                                {
                                    if (_DataSet.Tables[0].Rows.Count != 0)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                        {
                                            _Result.UserRating = _DataRow["Rating"] != DBNull.Value ? Convert.ToInt32(_DataRow["Rating"]) : 0;
                                            _Result.BookId = _DataRow["BookId"] != DBNull.Value ? Convert.ToInt32(_DataRow["BookId"]) : 0;
                                            _Result.IsReadLater = _DataRow["IsReadLater"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsReadLater"]) : false;
                                            _Result.HasReadAloud = _DataRow["HasReadAloud"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasReadAloud"]) : false;
                                            _Result.HasAnimation = _DataRow["HasAnimation"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasAnimation"]) : false;
                                            _Result.HasActivity = _DataRow["HasActivity"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasActivity"]) : false;
                                            _Result.IsRead = _DataRow["IsRead"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsRead"]) : false;
                                            _Result.IsReviewDone = _DataRow["IsReviewDone"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsReviewDone"]) : false;
                                            _Result.IsActivityDone = _DataRow["IsActivityDone"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsActivityDone"]) : false;
                                            _Result.Title = _DataRow["Title"] != DBNull.Value ? HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Title"]).Replace("\t", " ")) : "";
                                            _Result.Author = _DataRow["Author"] != DBNull.Value ? HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Author"]).Replace("\t", " ")) : "";
                                            _Result.Publisher = _DataRow["Publisher"] != DBNull.Value ? HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Publisher"]).Replace("\t", " ")) : "";
                                            _Result.Translator = _DataRow["Translator"] != DBNull.Value ? HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Translator"]).Replace("\t", " ")) : "";
                                            _Result.Illustrator = _DataRow["Illustrator"] != DBNull.Value ? HttpUtility.HtmlDecode(Convert.ToString(_DataRow["Illustrator"]).Replace("\t", " ")) : "";
                                            _Result.ShortDescription = _DataRow["ShortDescription"] != DBNull.Value ? HttpUtility.HtmlDecode(Convert.ToString(_DataRow["ShortDescription"]).Replace("\t", " ")) : "";
                                            _Result.Thumbnail3 = _DataRow["Thumbnail3"] != DBNull.Value ? Convert.ToString(_DataRow["Thumbnail3"]) : "";
                                            _Result.SubSection = _DataRow["SubSection"] != DBNull.Value ? Convert.ToInt32(_DataRow["SubSection"]) : 0;
                                            _Result.SubSectionName = _DataRow["SubSectionName"] != DBNull.Value ? Convert.ToString(_DataRow["SubSectionName"]) : "";
                                            _Result.Rating.AverageRating = _DataRow["AverageRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["AverageRating"]) : 0;
                                            _Result.Rating.TotalUserRatedThisBook = _DataRow["TotalUserRatedThisBook"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalUserRatedThisBook"]) : 0;
                                            _Result.Rating.OneStarRating = _DataRow["TotalOneRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalOneRating"]) : 0;
                                            _Result.Rating.TwoStarRating = _DataRow["TotalTwoRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalTwoRating"]) : 0;
                                            _Result.Rating.ThreeStarRating = _DataRow["TotalThreeRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalThreeRating"]) : 0;
                                            _Result.Rating.FourStarRating = _DataRow["TotalFourRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalFourRating"]) : 0;
                                            _Result.Rating.FiveStarRating = _DataRow["TotalFiveRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["TotalFiveRating"]) : 0;
                                            _Result.CurrentPage = _DataRow["CurrentPage"] != DBNull.Value ? Convert.ToInt32(_DataRow["CurrentPage"]) : 0;
                                            _Result.ViewMode = _DataRow["ViewMode"] != DBNull.Value ? Convert.ToString(_DataRow["ViewMode"]) : "";
                                        }
                                    }
                                    _Result.Status = BookStatus.Success;
                                }
                                else
                                    _Result.Status = BookStatus.NoBookExists;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetBookDetail", string.Format("Error occured while getting book {0} details for user {1}", bookId, userId), ex.InnerException.ToString(), BookStatus.Error.ToString());
                            _Result.Status = BookStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetBookDetail", string.Format("Error occured while getting book {0} details for user {1}", bookId, userId), ex.InnerException.ToString(), BookStatus.Error.ToString());
                _Result.Status = BookStatus.Error;
            }
            return _Result;
        }

        public BookDisplay BookDisplay(int userId, int bookId)
        {
            BookDisplay _Result = new BookDisplay();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spBookDisplay", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                        command.Parameters.Add("@bookId", SqlDbType.Int).Value = bookId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if ((int)id.Value == 1)
                            {
                                if (_DataSet.Tables.Count > 0)
                                {
                                    if (_DataSet.Tables[0].Rows.Count != 0)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                        {
                                            _Result.CurrentPage = _DataRow["CurrentPage"] != DBNull.Value ? Convert.ToInt32(_DataRow["CurrentPage"]) : 0;
                                            _Result.BookId = _DataRow["BookId"] != DBNull.Value ? Convert.ToInt32(_DataRow["BookId"]) : 0;
                                            _Result.KitabletID = _DataRow["KitabletID"] != DBNull.Value ? Convert.ToString(_DataRow["KitabletID"]) : "";
                                            _Result.PageDisplay = _DataRow["PageDisplay"] != DBNull.Value ? Convert.ToString(_DataRow["PageDisplay"]) : "";
                                            _Result.IsPagerAllowed = _DataRow["IsPagerAllowed"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsPagerAllowed"]) : false;
                                            _Result.HasReadAloud = _DataRow["HasReadAloud"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasReadAloud"]) : false;
                                            _Result.KitabletID = _DataRow["KitabletID"] != DBNull.Value ? Convert.ToString(_DataRow["KitabletID"]) : "";
                                        }
                                    }
                                    _Result.Status = GenericStatus.Sucess;
                                }
                                else
                                    _Result.Status = GenericStatus.Other;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "OpenBook", string.Format("Error occured while openning book {0} details for user {1}", bookId, userId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                            _Result.Status = GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "OpenBook", string.Format("Error occured while openning book {0} details for user {1}", bookId, userId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                _Result.Status = GenericStatus.Error;
            }
            return _Result;
        }

        public GenericStatus SetBookReading(int UserId, int BookId, bool IsCompleted, int CurrentPage, List<Page> page, string Environment, string Platform, DateTime StartDate, DateTime CompletedOn, string Json)
        {
            var xml = new XElement("list",
         from p in page
         select new XElement("page",
              new XElement("pageno", p.PageNumber),
             new XElement("endtime", p.EndTime),
             new XElement("starttime", p.StartTime)
             ));

            try
            {
                if (UserId > 0)
                    _currentUserId = UserId;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSetBookReading", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;
                        command.Parameters.Add("@IsCompleted", SqlDbType.Bit).Value = IsCompleted;
                        command.Parameters.Add("@CurrentPage", SqlDbType.Int).Value = CurrentPage;
                        command.Parameters.Add("@XML", SqlDbType.NVarChar).Value = xml.ToString();
                        command.Parameters.Add("@Environment", SqlDbType.NVarChar).Value = Environment;
                        command.Parameters.Add("@Platform", SqlDbType.NVarChar).Value = Platform;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        command.Parameters.Add("@CompletedOn", SqlDbType.DateTime).Value = CompletedOn;
                        command.Parameters.Add("@Data", SqlDbType.VarChar).Value = Json;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if ((int)id.Value == 1)
                            {
                                InsertLog(_currentUserId, "SetBookReading", string.Format(" Set book reading {0} progress details for user {1}", BookId, UserId), string.Format(" Set book reading {0} progress details for user {1}", BookId, UserId), GenericStatus.Sucess.ToString());
                                return GenericStatus.Sucess;
                            }
                            else
                            {
                                InsertLog(_currentUserId, "SetBookReading", string.Format("No book with id = {0} to set reading progress for user {1}", BookId, UserId), string.Format(" Set book reading {0} progress details for user {1}", BookId, UserId), GenericStatus.Sucess.ToString());
                                return GenericStatus.Other;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "SetBookReading", string.Format("Error occured while setting book reading {0} details for user {1}", BookId, UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                            return GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SetBookReading", string.Format("Error occured while setting book reading {0} details for user {1}", BookId, UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                return GenericStatus.Error;
            }

        }

        public GenericStatus SetBookActivity(BookActivity activity,string Json)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSetBookActivity", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = activity.UserId;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = activity.BookId;
                        command.Parameters.Add("@IsActivityDone", SqlDbType.Bit).Value = activity.IsActivityDone;
                        command.Parameters.Add("@Json", SqlDbType.NVarChar).Value = activity.Json;
                        command.Parameters.Add("@CompletedOn", SqlDbType.DateTime).Value = activity.CompletedOn;
                        command.Parameters.Add("@CompletionTime", SqlDbType.Int).Value = activity.CompletionTime;
                        command.Parameters.Add("@Environment", SqlDbType.NVarChar).Value = activity.Environment;
                        command.Parameters.Add("@Platform", SqlDbType.NVarChar).Value = activity.Platform;
                        command.Parameters.Add("@Data", SqlDbType.VarChar).Value = Json;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if ((int)id.Value == 1)
                            {
                                InsertLog(_currentUserId, "SetBookActivity", "Set activity completion", string.Format("Set book activity book = {0} details for user {1}", activity.BookId, activity.UserId), GenericStatus.Sucess.ToString());
                                return GenericStatus.Sucess;
                            }
                            else
                            {
                                InsertLog(_currentUserId, "SetBookActivity", "No book for setting activity", string.Format("No book with id= {0} to set activity for user {1}", activity.BookId, activity.UserId), GenericStatus.Other.ToString());
                                return GenericStatus.Other;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "SetBookActivity", string.Format("Error occured while setting book activity book =  {0} details for user {1}", activity.BookId, activity.UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                            return GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SetBookActivity", string.Format("Error occured while setting book activity book = {0}  details for user {1}", activity.BookId, activity.UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                return GenericStatus.Error;
            }

        }

        public BookActivity GetBookActivity(int UserId, int BookId)
        {
            BookActivity _result = new BookActivity();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetBookActivity", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            int status = (int)id.Value;
                            if (status == 0)
                                _result.Status = BookStatus.Error;
                            else if (status == 1)
                            {
                                if (_DataSet != null && _DataSet.Tables.Count > 0)
                                {
                                    _result.Json = _DataSet.Tables[0].Rows[0]["Json"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["Json"].ToString() : "";
                                    _result.HasAnimation = _DataSet.Tables[0].Rows[0]["HasAnimation"] != DBNull.Value ? Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["HasAnimation"]) : false;
                                    _result.HasReadAloud = _DataSet.Tables[0].Rows[0]["HasReadAloud"] != DBNull.Value ? Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["HasReadAloud"]) : false;
                                    _result.Rating = _DataSet.Tables[0].Rows[0]["Rating"] != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["Rating"]) : 0;
                                    _result.ViewMode = _DataSet.Tables[0].Rows[0]["ViewMode"] != DBNull.Value ? _DataSet.Tables[0].Rows[0]["ViewMode"].ToString() : "";
                                    _result.BookId = _DataSet.Tables[0].Rows[0]["BookId"] != DBNull.Value ? Convert.ToInt32(_DataSet.Tables[0].Rows[0]["BookId"]) : 0;
                                    _result.KitabletId = _DataSet.Tables[0].Rows[0]["KitabletId"] != DBNull.Value ? Convert.ToString(_DataSet.Tables[0].Rows[0]["KitabletId"]) : "";
                                    _result.IsActivityDone = _DataSet.Tables[0].Rows[0]["IsActivityDone"] != DBNull.Value ? Convert.ToBoolean(_DataSet.Tables[0].Rows[0]["IsActivityDone"]) : false;

                                }
                                _result.Status = BookStatus.Success;
                            }
                            else if (status == 2)
                                _result.Status = BookStatus.NoBookExists;
                            else if (status == 3)
                                _result.Status = BookStatus.BookNotRead;
                            else if (status == 4)
                                _result.Status = BookStatus.NoActivity;

                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetBookActivity", string.Format("Error occured while getting book activity book =  {0} details for user {1}", BookId, UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                            _result.Status = BookStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetBookActivity", string.Format("Error occured while getting book activity book = {0}  details for user {1}", BookId, UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                _result.Status = BookStatus.Error;
            }
            return _result;
        }

        public BookRead BookCompleted(int UserId, int BookId)
        {
            BookRead _Result = new BookRead();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spBookCompleted", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = BookId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if ((int)id.Value == 1)
                            {
                                if (_DataSet.Tables.Count > 0)
                                {
                                    if (_DataSet.Tables[0].Rows.Count != 0)
                                    {
                                        foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                        {
                                            _Result.UserRating = _DataRow["Rating"] != DBNull.Value ? Convert.ToInt32(_DataRow["Rating"]) : 0;
                                            _Result.BookId = _DataRow["BookId"] != DBNull.Value ? Convert.ToInt32(_DataRow["BookId"]) : 0;
                                            _Result.HasReadAloud = _DataRow["HasReadAloud"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasReadAloud"]) : false;
                                            _Result.HasAnimation = _DataRow["HasAnimation"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasAnimation"]) : false;
                                            _Result.HasActivity = _DataRow["HasActivity"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasActivity"]) : false;
                                            _Result.IsReviewDone = _DataRow["IsReviewDone"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsReviewDone"]) : false;
                                            _Result.IsActivityDone = _DataRow["IsActivityDone"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsActivityDone"]) : false;
                                            _Result.Thumbnail3 = _DataRow["Thumbnail3"] != DBNull.Value ? Convert.ToString(_DataRow["Thumbnail3"]) : "";
                                            _Result.SubSection = _DataRow["SubSection"] != DBNull.Value ? Convert.ToInt32(_DataRow["SubSection"]) : 0;
                                            _Result.SubSectionName = _DataRow["SubSectionName"] != DBNull.Value ? Convert.ToString(_DataRow["SubSectionName"]) : "";
                                            _Result.Rating.AverageRating = _DataRow["AverageRating"] != DBNull.Value ? Convert.ToInt32(_DataRow["AverageRating"]) : 0;
                                            _Result.ReviewJson = _DataRow["ReviewJson"] != DBNull.Value ? Convert.ToString(_DataRow["ReviewJson"]) : "";
                                            _Result.KitabletID = _DataRow["KitabletID"] != DBNull.Value ? Convert.ToString(_DataRow["KitabletID"]) : "";
                                            _Result.ViewMode = _DataRow["ViewMode"] != DBNull.Value ? Convert.ToString(_DataRow["ViewMode"]) : "";
                                        }
                                    }
                                    _Result.Status = BookStatus.Success;
                                }
                            }
                            else if ((int)id.Value == 2)
                                _Result.Status = BookStatus.NoBookExists;

                            else if ((int)id.Value == 3)
                                _Result.Status = BookStatus.BookNotRead;
                            else
                                _Result.Status = BookStatus.Error;
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "BookCompleted", string.Format("Error occured while getting book {0} details reviews for user {1}", BookId, UserId), ex.InnerException.ToString(), BookStatus.Error.ToString());
                            _Result.Status = BookStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "BookCompleted", string.Format("Error occured while getting book {0} details reviews for user {1}", BookId, UserId), ex.InnerException.ToString(), BookStatus.Error.ToString());
                _Result.Status = BookStatus.Error;
            }
            return _Result;
        }

        public GenericStatus SetBookReview(BookActivity review,string Json)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spSetBookReview", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = review.UserId;
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = review.BookId;
                        command.Parameters.Add("@IsReviewDone", SqlDbType.Bit).Value = review.IsActivityDone;
                        command.Parameters.Add("@Json", SqlDbType.NVarChar).Value = review.Json;
                        command.Parameters.Add("@CompletedOn", SqlDbType.DateTime).Value = review.CompletedOn;
                        command.Parameters.Add("@CompletionTime", SqlDbType.Int).Value = review.CompletionTime;
                        command.Parameters.Add("@Environment", SqlDbType.NVarChar).Value = review.Environment;
                        command.Parameters.Add("@Platform", SqlDbType.NVarChar).Value = review.Platform;
                        command.Parameters.Add("@Rating", SqlDbType.Int).Value = review.Rating;
                        command.Parameters.Add("@Data", SqlDbType.VarChar).Value = Json;

                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if ((int)id.Value == 1)
                            {
                                InsertLog(_currentUserId, "SetBookReview", "Set review completion", string.Format("Set book review book = {0} details for user {1}", review.BookId, review.UserId), GenericStatus.Sucess.ToString());
                                return GenericStatus.Sucess;
                            }
                            else
                            {
                                InsertLog(_currentUserId, "SetBookReview", "No book for setting review", string.Format("No book with id= {0} to set review for user {1}", review.BookId, review.UserId), GenericStatus.Other.ToString());
                                return GenericStatus.Other;
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "SetBookReview", string.Format("Error occured while setting book review book =  {0} details for user {1}", review.BookId, review.UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                            return GenericStatus.Error;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "SetBookReview", string.Format("Error occured while setting book review book = {0}  details for user {1}", review.BookId, review.UserId), ex.InnerException.ToString(), GenericStatus.Error.ToString());
                return GenericStatus.Error;
            }

        }

        public UAC GetUAC(int UserId)
        {
            UAC _Result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUAC", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();

                        try
                        {
                            _SqlDataAdapter.Fill(_DataSet);
                            if (_DataSet.Tables.Count > 0)
                            {
                                _Result = new UAC();
                                if (_DataSet.Tables[0].Rows.Count != 0)
                                {
                                    foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                                    {
                                        _Result.UserId = int.Parse(_DataRow["UserId"].ToString());
                                        _Result.TotalBookRead = int.Parse(_DataRow["TotalBookRead"].ToString());
                                        _Result.TotalBookRated = int.Parse(_DataRow["TotalBookRated"].ToString());
                                        _Result.TotalActivitiesCompleted = int.Parse(_DataRow["TotalActivitiesCompleted"].ToString());
                                        _Result.TotalHourSpent = double.Parse(_DataRow["TotalHourSpent"].ToString());
                                        _Result.LastAccessedBookId = int.Parse(_DataRow["LastAccessedBookId"].ToString());
                                        _Result.LastReadLaterBookId = int.Parse(_DataRow["LastReadLaterBookId"].ToString());
                                        _Result.TotalHourSpentOnActivity = int.Parse(_DataRow["TotalHourSpentOnActivity"].ToString());
                                        _Result.TotalHourSpentOnReading = int.Parse(_DataRow["TotalHourSpentOnReading"].ToString());
                                        _Result.TotalHourSpentOnReview = int.Parse(_DataRow["TotalHourSpentOnReview"].ToString());
                                    }
                                }

                                if (_DataSet.Tables[1].Rows.Count != 0)
                                {

                                    foreach (DataRow _DataRow in _DataSet.Tables[1].Rows)
                                    {
                                        _Result.BookId = _DataRow["BookId"] != DBNull.Value ? Convert.ToInt32(_DataRow["BookId"]) : 0;
                                        _Result.Rating = _DataRow["Rating"] != DBNull.Value ? Convert.ToInt32(_DataRow["Rating"]) : 0;
                                        _Result.IsActivityDone = _DataRow["IsActivityDone"] != DBNull.Value ? Convert.ToBoolean(_DataRow["IsActivityDone"]) : false;
                                        _Result.HasAnimation = _DataRow["HasAnimation"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasAnimation"]) : false;
                                        _Result.HasActivity = _DataRow["HasActivity"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasActivity"]) : false;
                                        _Result.HasReadAloud = _DataRow["HasReadAloud"] != DBNull.Value ? Convert.ToBoolean(_DataRow["HasReadAloud"]) : false;
                                        _Result.ViewMode = _DataRow["ViewMode"] != DBNull.Value ? Convert.ToString(_DataRow["ViewMode"]) : "";
                                        _Result.Thumbnail2 = _DataRow["Thumbnail2"] != DBNull.Value ? Convert.ToString(_DataRow["Thumbnail2"]) : "";
                                        _Result.SubSectionId = _DataRow["SubSectionId"] != DBNull.Value ? Convert.ToInt32(_DataRow["SubSectionId"]) : 0;
                                    }
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetUAC", string.Format("Error occured while getting book details for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetUAC", string.Format("Error occured while getting book details for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return _Result;
        }

        public bool UpdateBrowsingTime(int UserId, int TimeSpent,DateTime EndDate,string Plateform,string Environment,string SessionId,string Callfrom)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateBrowsingTime", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        command.Parameters.Add("@TimeSpent", SqlDbType.Int).Value = TimeSpent;
                        command.Parameters.Add("@PlateForm", SqlDbType.VarChar).Value = Plateform;
                        command.Parameters.Add("@Environment", SqlDbType.VarChar).Value = Environment;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        command.Parameters.Add("@SessionId", SqlDbType.VarChar).Value = SessionId;
                        command.Parameters.Add("@Callfrom", SqlDbType.VarChar).Value = Callfrom;

                        try
                        {
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            if (command.ExecuteNonQuery() > 0)
                                return true;
                            return false;
                        }
                        catch (SqlException ex)
                        {
                            InsertLog(_currentUserId, "GetUAC", string.Format("Error occured while updateting browsing time for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertLog(_currentUserId, "GetUAC", string.Format("Error occured while updateting browsing time for user {0}", UserId), ex.InnerException.ToString(), UserStatus.Error.ToString());
            }
            return false;
        }

    }
}