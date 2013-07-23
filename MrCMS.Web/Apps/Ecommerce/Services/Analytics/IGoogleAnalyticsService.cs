using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics
{
    public interface IGoogleAnalyticsService
    {
        string GetAnalayticsCode(Order order, string trackingScript);
    }
}