using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FISE_API.Services.EmailService
{
    public class UserTokenEmailService
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["FISE_APIConString"].ConnectionString;

        public void UpdateUserToken(int Id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateUserToken", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {

            }
        }

        public List<UserToken> SearchUserToken()
        {
            var userTokens = new List<UserToken>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUserTokens", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        _SqlDataAdapter.Fill(_DataSet);
                        if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                UserToken _UserToken = new UserToken();
                                _UserToken.Id = int.Parse(_DataRow["Id"].ToString());
                                _UserToken.UserId = int.Parse(_DataRow["EntityId"].ToString());
                                _UserToken.Token = _DataRow["Token"].ToString();
                                _UserToken.Type = _DataRow["Type"].ToString();
                                _UserToken.Email = _DataRow["Email"].ToString();
                                _UserToken.FirstName = _DataRow["FirstName"].ToString();
                                userTokens.Add(_UserToken);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return userTokens;
        }

        public DataSet GetUserNameRecovery(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetUserNameRecovery", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Email", email);
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        _SqlDataAdapter.Fill(_DataSet);
                        return _DataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                return null;
            }
        }

        public DataSet GetImportDetails(int UserId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetImportDetails", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        _SqlDataAdapter.Fill(_DataSet);
                        return _DataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                return null;
            }
        }

    }
}
