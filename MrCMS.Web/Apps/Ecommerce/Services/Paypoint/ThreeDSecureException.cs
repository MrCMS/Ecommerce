using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    [Serializable]
    public sealed class ThreeDSecureException : Exception
    {
        public ThreeDSecureException()
        {
        }

        public ThreeDSecureException(NameValueCollection error)
            : base("3D Secure error")
        {
            Data["Request Data"] = string.Join("&", error.AllKeys.Select(s => string.Format("{0}={1}", s, error[s])));
        }

        private ThreeDSecureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}