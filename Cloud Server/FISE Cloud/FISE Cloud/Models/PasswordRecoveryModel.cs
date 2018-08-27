using System.Web.Mvc;

namespace FISE_Cloud.Models
{
    public class PasswordRecoveryModel
    {
        [AllowHtml]
        public string Username { get; set; }
        public string Result { get; set; }
        public bool SuccessfullySent { get; set; }
        public bool IsTokenValid { get; set; }
    }

}