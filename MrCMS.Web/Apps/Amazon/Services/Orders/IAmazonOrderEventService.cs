using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderEventService
    {
        void AmazonOrderPlaced(AmazonOrder order);
    }
}