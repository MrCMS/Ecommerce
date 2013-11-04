using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IBackInStockNotificationService
    {
        void SaveRequest(BackInStockNotificationRequest request);
    }
}