using System.Xml.Serialization;

namespace FISE_API.Services.EmailService
{

    public class MessageTemplate
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
        [XmlElement("Body")]
        public string Body { get; set; }
        [XmlElement("Subject")]
        public string Subject { get; set; }        
        [XmlElement("EmailAccountId")]
        public int EmailAccountId { get; set; }
        [XmlElement("IsActive")]
        public bool IsActive { get; set; }       
        
    }
}