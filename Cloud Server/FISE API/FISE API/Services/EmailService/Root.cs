using System.Collections.Generic;
using System.Xml.Serialization;

namespace FISE_API.Services.EmailService
{
    [XmlRoot("EmailAccounts")]
    public class Root
    {
        public Root() {
            EmailAccounts = new List<EmailAccounts>();
            MessageTemplates = new List<MessageTemplates>();
        }
        [XmlElement("MessageTemplates")]
        public List<MessageTemplates> MessageTemplates { get; set; }
        [XmlElement("EmailAccounts")]
        public List<EmailAccounts> EmailAccounts { get; set; }
    }
   
    public class EmailAccounts
    {
        public EmailAccounts() { Accounts = new List<EmailAccount>(); }
        [XmlElement("EmailAccount")]
        public List<EmailAccount> Accounts { get; set; }
    }
    public class MessageTemplates
    {
        public MessageTemplates() { Templates = new List<MessageTemplate>(); }
        [XmlElement("MessageTemplate")]
        public List<MessageTemplate> Templates { get; set; }
    }
}