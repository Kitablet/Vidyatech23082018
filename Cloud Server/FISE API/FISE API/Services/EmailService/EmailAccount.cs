using System.Xml.Serialization;

namespace FISE_API.Services.EmailService
{

    public class EmailAccount
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [XmlElement("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets an email address
        /// </summary>
         [XmlElement("Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets an email display name
        /// </summary>
        [XmlElement("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets an email host
        /// </summary>
         [XmlElement("Host")]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets an email port
        /// </summary>
         [XmlElement("Port")]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets an email user name
        /// </summary>
         [XmlElement("Username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets an email password
        /// </summary>
         [XmlElement("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection
        /// </summary>
        [XmlElement("EnableSsl")]
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the default system credentials of the application are sent with requests.
        /// </summary>
         [XmlElement("UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }
    }
}