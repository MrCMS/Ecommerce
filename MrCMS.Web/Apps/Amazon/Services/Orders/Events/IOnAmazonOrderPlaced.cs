using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Events
{
    public interface IOnAmazonOrderPlaced : IAmazonOrderEvent
    {
        void IOnAmazonOrderPlaced(AmazonOrder order);
    }
}