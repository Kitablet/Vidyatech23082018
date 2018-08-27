using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FISE_API.Services.EmailService
{
    public class QueuedEmailService
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["FISE_APIConString"].ConnectionString;
            
        public bool InsertQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");            
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spInsertQueuedEmail", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Priority", SqlDbType.Int).Value = queuedEmail.Priority;
                        command.Parameters.Add("@EmailFrom", SqlDbType.NVarChar).Value = queuedEmail.From;
                        command.Parameters.Add("@FromName", SqlDbType.NVarChar).Value = queuedEmail.FromName;
                        command.Parameters.Add("@EmailTo", SqlDbType.NVarChar).Value = queuedEmail.To;
                        command.Parameters.Add("@ToName", SqlDbType.NVarChar).Value = queuedEmail.ToName;
                        command.Parameters.Add("@ReplyTo", SqlDbType.NVarChar).Value = queuedEmail.ReplyTo;
                        command.Parameters.Add("@ReplyToName", SqlDbType.NVarChar).Value = queuedEmail.ReplyToName;
                        command.Parameters.Add("@CC", SqlDbType.NVarChar).Value = queuedEmail.CC;
                        command.Parameters.Add("@Bcc", SqlDbType.NVarChar).Value = queuedEmail.Bcc;
                        command.Parameters.Add("@Subject", SqlDbType.NVarChar).Value = queuedEmail.Subject;
                        command.Parameters.Add("@Body", SqlDbType.NVarChar).Value = queuedEmail.Body;
                        command.Parameters.Add("@AttachmentFilePath", SqlDbType.NVarChar).Value = queuedEmail.AttachmentFilePath;
                        command.Parameters.Add("@AttachmentFileName", SqlDbType.NVarChar).Value = queuedEmail.AttachmentFileName;
                        command.Parameters.Add("@EmailAccountId", SqlDbType.Int).Value = queuedEmail.EmailAccountId;
                        command.Parameters.Add("@TokenId", SqlDbType.Int).Value = queuedEmail.TokenId;
                        SqlParameter id = new SqlParameter("@Status", SqlDbType.Int);
                        id.Direction = ParameterDirection.Output;
                        command.Parameters.Add(id);
                        if (con.State == ConnectionState.Open)
                            con.Close();
                        con.Open();
                        command.ExecuteNonQuery();
                        int Status = (int)id.Value;
                        if (Status == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
       
        public void UpdateQueuedEmail(int Id,bool IsSent)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateQueuedEmail ", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                        command.Parameters.Add("@IsSent", SqlDbType.Bit).Value = IsSent;
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


        public List<QueuedEmail> SearchEmails(int maxSendTries, bool loadNewest)
        {
            var queuedEmails = new List<QueuedEmail>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("spGetQueuedEmail", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@maxSendTries", SqlDbType.Int).Value = maxSendTries;
                        command.Parameters.Add("@loadNewest", SqlDbType.Bit).Value = loadNewest;
                        SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(command);
                        DataSet _DataSet = new DataSet();
                        _SqlDataAdapter.Fill(_DataSet);
                        if (_DataSet.Tables[0] != null && _DataSet.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow _DataRow in _DataSet.Tables[0].Rows)
                            {
                                QueuedEmail _QueuedEmail = new QueuedEmail();
                                _QueuedEmail.Id = int.Parse(_DataRow["Id"].ToString());
                                _QueuedEmail.Priority = int.Parse(_DataRow["Priority"].ToString());
                                _QueuedEmail.From = _DataRow["EmailFrom"].ToString();
                                _QueuedEmail.FromName = _DataRow["FromName"].ToString();
                                _QueuedEmail.To = _DataRow["EmailTo"].ToString();
                                _QueuedEmail.ToName = _DataRow["ToName"].ToString();
                                _QueuedEmail.ReplyTo = _DataRow["ReplyTo"].ToString();
                                _QueuedEmail.ReplyToName = _DataRow["ReplyToName"].ToString();
                                _QueuedEmail.CC = _DataRow["CC"].ToString();
                                _QueuedEmail.Bcc = _DataRow["Bcc"].ToString();
                                _QueuedEmail.Subject = _DataRow["Subject"].ToString();
                                _QueuedEmail.Body = _DataRow["Body"].ToString();
                                _QueuedEmail.AttachmentFilePath = _DataRow["AttachmentFilePath"].ToString();
                                _QueuedEmail.AttachmentFileName = _DataRow["AttachmentFileName"].ToString();
                                _QueuedEmail.EmailAccountId = int.Parse(_DataRow["EmailAccountId"].ToString());
                                _QueuedEmail.SentTries = int.Parse(_DataRow["SentTries"].ToString());
                                _QueuedEmail.CreatedOnUtc = DateTime.Parse(_DataRow["CreatedOnUtc"].ToString());                                
                                queuedEmails.Add(_QueuedEmail);
                            }
                        }

                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return queuedEmails;
        }
    }
}
