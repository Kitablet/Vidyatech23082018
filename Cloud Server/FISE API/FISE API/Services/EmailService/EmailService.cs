using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace FISE_API.Services.EmailService
{
    /// <summary>
    /// Email sender
    /// </summary>
    public class EmailService
    {
        public void SendEmail(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            var message = new MailMessage();
            //from, to, reply to
            message.From = new MailAddress(fromAddress, fromName);

            message.To.Add(new MailAddress(toAddress, toName));

            if (!String.IsNullOrEmpty(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
            }

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            //CC
            //if (cc != null)
            //{
            //    foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
            //    {
            //        message.CC.Add(address.Trim());
            //    }
            //}
            //send custom cc from config
            string CC = System.Configuration.ConfigurationManager.AppSettings["EmailCC"];
            if (!string.IsNullOrEmpty(CC))
            {
                foreach (var address in CC.Split(',').Where(ccvalue => !string.IsNullOrWhiteSpace(ccvalue)))
                {
                    message.CC.Add(address.Trim());
                }
            }
            //EmailBCC
            string BCC = System.Configuration.ConfigurationManager.AppSettings["EmailBCC"];
            if (!string.IsNullOrEmpty(BCC))
            {
                foreach (var address in BCC.Split(',').Where(bccvalue => !string.IsNullOrWhiteSpace(bccvalue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }
            //content
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            //create  the file attachment for this e-mail message
            if (!String.IsNullOrEmpty(attachmentFilePath) &&
                File.Exists(attachmentFilePath))
            {
                var attachment = new Attachment(attachmentFilePath);
                attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentFilePath);
                attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentFilePath);
                attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentFilePath);
                if (!String.IsNullOrEmpty(attachmentFileName))
                {
                    attachment.Name = attachmentFileName;
                }
                message.Attachments.Add(attachment);
            }
            //send email
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                    smtpClient.Host = emailAccount.Host;
                    smtpClient.Port = emailAccount.Port;
                    smtpClient.EnableSsl = emailAccount.EnableSsl;
                    if (emailAccount.UseDefaultCredentials)
                        smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                    else
                        smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);
                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    string log = ex.Message + " occured at " + DateTime.Now.ToString() + " while sending mail.\r\n";
                    File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~\\App_Data\\EmailErrorLog.txt"), log);
                }
            }
        }
    }
}
