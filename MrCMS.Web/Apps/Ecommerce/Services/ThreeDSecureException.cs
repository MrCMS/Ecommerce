using System;
using System.Collections.Specialized;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ThreeDSecureException : Exception
    {
        public ThreeDSecureException()
        {
        }

        public ThreeDSecureException(NameValueCollection error)
            : base("3D Secure error")
        {
            string.Join("&", error.AllKeys.Select(s => string.Format("{0}={1}", s, error[(string) s])));
        }

        public string RequestData { get; set; }
    }
}