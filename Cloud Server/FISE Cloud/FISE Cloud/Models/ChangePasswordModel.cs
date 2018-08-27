using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FISE_Cloud.Models
{
    public class ChangePasswordModel
    {
        [AllowHtml]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmNewPassword { get; set; }

        public string Result { get; set; }
        public bool Status { get; set; }
    }
}