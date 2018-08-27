using System;
using System.IO;
using System.Configuration;
using FISE_API.Services.EmailService;
using System.Data;
using System.Text;
namespace FISE_API.Tasks
{
    public class EmailTask : ITask
    {
        public void Execute()
        {

            var maxTries = 3;
            bool IsSent = false;
            QueuedEmailService _queuedEmailService = new QueuedEmailService();
            var queuedEmails = _queuedEmailService.SearchEmails(maxTries, false);
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = String.IsNullOrWhiteSpace(queuedEmail.Bcc)
                            ? null
                            : queuedEmail.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = String.IsNullOrWhiteSpace(queuedEmail.CC)
                            ? null
                            : queuedEmail.CC.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    EmailAccount account = new MessageTemplateService().GetEmailAccountById(queuedEmail.EmailAccountId);
                    new EmailService().SendEmail(account,
                        queuedEmail.Subject,
                        queuedEmail.Body,
                       queuedEmail.From,
                       queuedEmail.FromName,
                       queuedEmail.To,
                       queuedEmail.ToName,
                       queuedEmail.ReplyTo,
                       queuedEmail.ReplyToName,
                       bcc,
                       cc,
                       queuedEmail.AttachmentFilePath,
                       queuedEmail.AttachmentFileName);

                    IsSent = true;
                }
                catch (Exception exc)
                {
                    string log = exc.Message + " occured at " + DateTime.Now.ToString() + " in email task.\r\n";
                    File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
                    IsSent = false;
                }
                finally
                {
                    _queuedEmailService.UpdateQueuedEmail(queuedEmail.Id, IsSent);
                }
            }

        }

    }

    public class UserTokenTask : ITask
    {
        public void Execute()
        {
            UserTokenEmailService _userTokenEmailService = new UserTokenEmailService();
            var queuedTokens = _userTokenEmailService.SearchUserToken();
            var _siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
            DataSet _dataset = new DataSet();
            foreach (var queuedToken in queuedTokens)
            {
                var Firstname = "User";
                if(!String.IsNullOrEmpty(queuedToken.FirstName))
                {
                    Firstname = queuedToken.FirstName;
                }
                switch (queuedToken.Type.ToLower())
                {
                    case "parentregistration":
                        if (EmailSender.SendParentRegistrationEmail(queuedToken.Token,queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        break;

                    case "studentregistration":
                        if (EmailSender.SendParentRegistrationEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        break;

                    case "passwordrecovery":
                        if (EmailSender.SendPasswordRecoveryEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;

                    case "schooladminregistration":
                        if (EmailSender.SendSchoolAdminRegistrationEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;

                    case "elibadminregistration":
                        if (EmailSender.SendElibAdminRegistrationEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;

                    case "usernamerecovery":
                        _dataset = _userTokenEmailService.GetUserNameRecovery(queuedToken.Email);
                        if (_dataset != null && _dataset.Tables.Count > 0)
                        {
                            if (EmailSender.SendUserNameRecoveryEmail(_dataset.Tables[0].Rows[0]["Username"].ToString(), queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                            {
                                _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                            }
                        }
                        break;
                    case "parentusernamerecovery":
                        _dataset = _userTokenEmailService.GetUserNameRecovery(queuedToken.Email);

                        if (_dataset != null && _dataset.Tables.Count > 0)
                        {
                            StringBuilder _str = new StringBuilder();
                            _str.Append("<table style=\"border-collapse: collapse;\"><tr><th style=\"border:1px solid black;padding:10px\"> First name</th><th style=\"border:1px solid black;padding:10px\">Last name</th><th style=\"border:1px solid black;padding:10px\">Username</th></tr>");
                            foreach (DataRow _row in _dataset.Tables[0].Rows)
                            {
                                _str.Append("<tr><td style=\"border:1px solid black;padding:10px\">" + _row["FirstName"] + "</td><td style=\"border:1px solid black;padding:10px\">" + _row["LastName"] + "</td><td style=\"border:1px solid black;padding:10px\">" + _row["UserName"] + "</td></tr>");
                            }
                            _str.Append("</table>");
                            if (EmailSender.SendParentUserNameRecoveryEmail(_str.ToString(), queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                            {
                                _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                            }
                        }
                        break;
                    case "schoolregistration":
                        if (EmailSender.SendSchoolRegistrationEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;
                    case "newstudentadded":
                        if (EmailSender.SendNewstudentAddedEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;

                    case "studentparentacknowledgement":
                        if (EmailSender.SendStudentParentAcknowledgementEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;

                    case "useremailchange":
                        if (EmailSender.SendUserEmailChangeVerificationEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;
                    case "schoolstudentspasswordrecoveryemail":
                        if (EmailSender.SendSchoolStudentsPasswordRecoveryEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;
                    case "studentregistration_parentregistered":
                        if (EmailSender.SendStudentRegistrationEmail(queuedToken.Token, queuedToken.Email, _siteUrl, Firstname, queuedToken.Id))
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        break;
                        
                    case "studentimport":
                    if (EmailSender.SendStudentImportEmail(queuedToken.Token, queuedToken.Email, queuedToken.UserId, _siteUrl, Firstname, queuedToken.Id))
                    {
                        _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                    }
                    break;
                    case "studentimport_parent":
                        if (EmailSender.SendStudentParentImportEmail(queuedToken.Token, queuedToken.Email, queuedToken.UserId, _siteUrl, Firstname, queuedToken.Id))
                        {
                            _userTokenEmailService.UpdateUserToken(queuedToken.Id);
                        }
                        break;
                }
            }
        }
    }
}