namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public interface IHttpRequestSender
    {
        /// <summary>
        /// Sends some data to a URL using an HTTP POST.
        /// </summary>
        /// <param name="url">Url to send to</param>
        /// <param name="postData">The data to send</param>
        string SendRequest(string url, string postData);
    }
}