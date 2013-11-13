using System;
using System.IO;
using System.Net;
using System.Text;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    /// <summary>
    /// Default implementation of IHttpRequestSender
    /// </summary>
    public class HttpRequestSender : IHttpRequestSender
    {
        /// <summary>
        /// Sends some data to a URL using an HTTP POST.
        /// </summary>
        /// <param name="url">Url to send to</param>
        /// <param name="postData">The data to send</param>
        public string SendRequest(string url, string postData)
        {
            var uri = new Uri(url);
            var request = WebRequest.Create(uri);
            var encoding = new UTF8Encoding();
            var requestData = encoding.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Timeout = (300 * 1000); //TODO: Move timeout to config
            request.ContentLength = requestData.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(requestData, 0, requestData.Length);
            }

            var response = request.GetResponse();

            string result;

            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}