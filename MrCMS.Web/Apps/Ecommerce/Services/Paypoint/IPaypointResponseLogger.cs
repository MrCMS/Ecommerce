using System.Collections.Specialized;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    public interface IPaypointResponseLogger
    {
        void LogResponse(string rawData, NameValueCollection collection);
    }
}