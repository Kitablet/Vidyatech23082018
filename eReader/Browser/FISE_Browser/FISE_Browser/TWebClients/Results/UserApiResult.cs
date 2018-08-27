using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FISE_Browser.Models;

namespace FISE_Browser.TWebClients.Results
{
    public class UserApiResult : ApiResult
    {
        public User User { get; set; }
    }
}