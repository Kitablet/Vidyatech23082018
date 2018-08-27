using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FISE_Browser.Models
{
    public class UserLogin
    {
        [AllowHtml]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [AllowHtml]
        public string Password { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool HasError { get; set; }
    }
}