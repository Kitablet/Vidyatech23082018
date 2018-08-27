using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FISE_Cloud.Models
{
    public class PasswordRecoveryConfirmModel
    {
        [AllowHtml]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmNewPassword { get; set; }

        public bool SuccessfullyChanged { get; set; }
        public string Result { get; set; }
        public string Username { get; set; }
    }
}