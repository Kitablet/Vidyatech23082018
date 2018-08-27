using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FISE_Cloud.Models
{
    public class LoginModel
    {
        [AllowHtml]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [AllowHtml]
        public string Password { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool HasError { get; set; }
        public bool ShowRegMsg { get; set; }
                
        public int AttemptCount { get; set; }
    }
}