using System;
using System.Web.Mvc;

namespace FISE_Cloud.CustomResult
{
    public class FiseHttpUnauthorisedResult: HttpUnauthorizedResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
        }

    }
}