using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;


namespace FISE_API.Services.EmailService
{
    public static class EmailSender
    {
        private static string _userImportPassword = System.Configuration.ConfigurationManager.AppSettings["UserImportPassword"];
        public static bool SendSchoolAdminRegistrationEmail(string Token,  string TO, string Url, string MemberName = null,int TokenId=0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("SchoolAdmin.Registration.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "register/" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendElibAdminRegistrationEmail(string Token,  string TO, string Url, string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("ElibAdmin.Registration.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);
                string TOKENURL = Url + "register/" + Token;
                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendPasswordRecoveryEmail(string Token,  string TO, string Url, string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("User.Password.Recovery");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "passwordrecovery/confirm?token=" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };

                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                string mee = ex.InnerException.Message;
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }
        
        public static bool SendParentRegistrationEmail(string Token,  string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Parent.Registration.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "register/" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = "",//MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendUserNameRecoveryEmail(string Token,  string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Username.Recovery.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = "",//MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendParentUserNameRecoveryEmail(string Token,  string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("ParentUsername.Recovery.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = "",//MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendSchoolRegistrationEmail(string Token,  string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("School.Registration.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "register/" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendNewstudentAddedEmail(string Token,  string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Student.Added.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

               // string TOKENURL = Url + "register/" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));

                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }


        public static bool SendStudentParentAcknowledgementEmail(string Token,  string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("StudentParent.Acknowledgement.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "register/" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendHelpAcknowledgementEmail(string Token,  string UserEmail, string Query, string CreatedOn, string Role,string StudentFirstName = "", string StudentLastName = "", string Url = "", string AdminEmail = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            string ToEmail = UserEmail;
            try
            {
                var tokens = new List<Token>();
                tokens.Add(new Token("SITEURL", Url));
                if (String.IsNullOrEmpty(AdminEmail))
                {
                    if (Role.ToLower() == "student")
                    {
                        messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Help.Acknowledgement.Student.Email");
                        tokens.Add(new Token("STUDENTFIRSTNAME", StudentFirstName));
                        tokens.Add(new Token("STUDENTLASTNAME", StudentLastName));
                    }
                    else
                    {
                        messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Help.Acknowledgement.OtherUser.Email");
                    }
                }
                else
                {
                    messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Help.Acknowledgement.AdminEmail");
                    ToEmail = AdminEmail;
                    tokens.Add(new Token("USEREMAIL", UserEmail));
                }
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);
                
                tokens.Add(new Token("REFERENCEID", Token));
                tokens.Add(new Token("QUERY", Query));
                tokens.Add(new Token("QUERYDATE", CreatedOn));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = ToEmail,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + ToEmail + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendUserEmailChangeVerificationEmail(string Token,  string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("User.EmailVerification.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "register/" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendSchoolStudentsPasswordRecoveryEmail(string Token,  string TO, string Url, string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("SchoolStudent.Password.Recovery");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "passwordrecovery/student?token=" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };

                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                string mee = ex.InnerException.Message;
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }

        public static bool SendStudentRegistrationEmail(string Token, string TO, string Url = "", string MemberName = null, int TokenId = 0)
        {
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Student.Registration.Email");
                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                string TOKENURL = Url + "register/" + Token;

                var tokens = new List<Token>();
                tokens.Add(new Token("TOKENURL", TOKENURL));
                tokens.Add(new Token("SITEURL", Url));
                tokens.Add(new Token("FIRSTNAME", MemberName));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = "",//MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;
        }


        public static bool SendStudentParentImportEmail(string Token, string TO, int UserId, string Url = "", string MemberName = null, int TokenId = 0)
        {
            UserTokenEmailService _userTokenEmailService = new UserTokenEmailService();
            DataSet ds = _userTokenEmailService.GetImportDetails(UserId);
            string USERNAME = string.Empty;
            string PASSWORD = _userImportPassword;
            if (ds != null)
            {
                if (ds.Tables.Count != 0)
                {
                    USERNAME=ds.Tables[0].Rows[0]["UserName"].ToString().ToLower();
                }
            }
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {             
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Parent.Import.Email");

                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                var tokens = new List<Token>();                
                tokens.Add(new Token("PARENTUSERNAME", USERNAME));
                tokens.Add(new Token("PARENTPASSWORD", PASSWORD));
                tokens.Add(new Token("SITEURL", Url));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = "",//MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;

        }
        public static bool SendStudentImportEmail(string Token, string TO, int UserId, string Url = "", string MemberName = null, int TokenId = 0)
        {

            UserTokenEmailService _userTokenEmailService = new UserTokenEmailService();
            DataSet ds = _userTokenEmailService.GetImportDetails(UserId);
            string USERNAME = string.Empty;
            string PASSWORD = _userImportPassword;
            if (ds != null)
            {
                if (ds.Tables.Count != 0)
                {
                    USERNAME = ds.Tables[0].Rows[0]["UserName"].ToString().ToLower();
                }
            }
            bool blnSent = false;
            MessageTemplateService _MessageTemplateService = new MessageTemplateService();
            MessageTemplate messageTemplate = null;
            Tokenizer _tokenizer = new Tokenizer(false);
            try
            {
                messageTemplate = _MessageTemplateService.GetMessageTemplateByName("Student.Import.Email");

                if (messageTemplate == null)
                    throw new ArgumentNullException("messageTemplate");
                //email account
                var emailAccount = _MessageTemplateService.GetEmailAccountById(messageTemplate.EmailAccountId);

                var tokens = new List<Token>();
                tokens.Add(new Token("STUDENTUSERNAME", USERNAME));
                tokens.Add(new Token("STUDENTPASSWORD", PASSWORD));
                tokens.Add(new Token("SITEURL", Url));
                var subject = messageTemplate.Subject;
                var body = messageTemplate.Body;
                //Replace subject and body tokens 
                var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
                var bodyReplaced = _tokenizer.Replace(body, tokens, false);
                QueuedEmail _QueuedEmail = new QueuedEmail
                {
                    Priority = 1,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = TO,
                    ToName = "",//MemberName,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    TokenId = TokenId
                };
                blnSent = new QueuedEmailService().InsertQueuedEmail(_QueuedEmail);
            }
            catch (Exception ex)
            {
                var log = ex.InnerException + " occured while sending mail to " + TO + " at " + DateTime.Now + "\n";
                File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
            }
            finally
            {
                _MessageTemplateService = null;
                messageTemplate = null;
                _tokenizer = null;
            }
            return blnSent;            
        }        
    }
}