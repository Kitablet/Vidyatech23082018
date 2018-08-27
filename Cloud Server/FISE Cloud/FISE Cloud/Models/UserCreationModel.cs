using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FISE_Cloud.Models
{
    public class UserCreationModel
    {
        public UserCreationModel()
        {
            Result = null;
        }
        [AllowHtml]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [AllowHtml]
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }
        public string GivenMobileNo { get; set; }
        public string Result { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public string MobileNumber { get; set; }
        public bool IsInvalidToken { get; set; }
        public bool AccountIsDisabled { get; set; }
        public String EmailDisplayName
        {
            get
            {
                String name = "";
                if (Type =="schooladmin")
                {
                    name = "Email:";
                }
                else if (Type == "elibraryadmin")
                {
                    name = "Email :";
                }
                else
                {
                    name = "Email :";
                }

                return name;
            }
        }
        public String MobileDisplayName
        {
            get
            {
                String name = "";
                if (Type == "schooladmin")
                {
                    name = "Mobile:";
                }
                else if (Type == "elibraryadmin")
                {
                    name = "Mobile :";
                }
                else
                {
                    name = "Mobile:";
                }

                return name;
            }
        }
    }
}