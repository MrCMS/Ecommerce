using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace MrCMS.Website
{
    public class OutOfContextResponse : HttpResponseBase
    {
        private readonly HttpCookieCollection _cookies;
        private readonly Stream _outputStream;
        private readonly NameValueCollection _headers;

        public OutOfContextResponse(HttpCookieCollection cookies = null)
        {
            _outputStream = new MemoryStream();
            _headers = new NameValueCollection();
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

        public override Stream OutputStream
        {
            get { return _outputStream; }
        }
        public override bool Buffer { get; set; }

        public override void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }
        public override NameValueCollection Headers
        {
            get { return _headers; }
        }
    }
}