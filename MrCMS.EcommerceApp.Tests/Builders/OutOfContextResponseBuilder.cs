using System.Web;
using MrCMS.Website;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class OutOfContextResponseBuilder
    {
        public OutOfContextResponse Build()
        {
            return new OutOfContextResponse(new HttpCookieCollection());
        }
    }
}