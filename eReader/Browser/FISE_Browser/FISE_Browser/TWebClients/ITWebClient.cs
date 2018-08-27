using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FISE_Browser.TWebClients
{
    public partial interface ITWebClient
    {
        T UploadData<T>(string resourceName, object postData);
        T DownloadData<T>(string resourceName,object param);
    }
}