using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FISE_Cloud.Caching
{
    public partial class CacheKey
    {
         /// <summary>
        /// Key for User caching
        /// </summary>
        /// <remarks>
        /// {0} : User id
        /// </remarks>
        public const string USER_KEY = "FISE_Cloud.User-{0}";
       
    }
}