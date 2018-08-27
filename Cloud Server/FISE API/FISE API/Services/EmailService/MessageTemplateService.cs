using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace FISE_API.Services.EmailService
{
    public class MessageTemplateService
    {
        public string  CONSTFILEPATH { get; set; }
        Root _Root;
        public MessageTemplateService()
        {
            CONSTFILEPATH = HostingEnvironment.MapPath(@"~/Services/EmailService/Resources/EmailTemplates.xml");
            try
            {
                using (var reader = new StreamReader(CONSTFILEPATH))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Root),
                        new XmlRootAttribute("Root"));
                    _Root = (Root)deserializer.Deserialize(reader);
                }
            }
            catch(Exception ex)
            {
                string cc = ex.InnerException.Message;
            }
        }
        public virtual MessageTemplate GetMessageTemplateByName(string messageTemplateName)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException("messageTemplateName");
            var query = _Root.MessageTemplates.FirstOrDefault().Templates.Where(t => t.Name == messageTemplateName && t.IsActive==true);
            var templates = query.ToList();
            return templates.FirstOrDefault();
        }

        public virtual EmailAccount GetEmailAccountById(int EmailAccountId)
        {
            var query = _Root.EmailAccounts.FirstOrDefault().Accounts.Where(t => t.Id == EmailAccountId);
            var templates = query.ToList();
            return templates.FirstOrDefault();
        }
               
    }
}