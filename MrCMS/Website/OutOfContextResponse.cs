using System.Web;

namespace MrCMS.Website
{
    public class OutOfContextResponse : HttpResponseBase
    {
        private readonly HttpCookieCollection _cookies;

        public OutOfContextResponse(HttpCookieCollection cookies=null)
        {
            _cookies = cookies ?? new HttpCookieCollection();
        }

        public override void Clear()
        {
        }
        public override void End()
        {
        }
        public override int StatusCode { get; set; }
        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }
    }
}