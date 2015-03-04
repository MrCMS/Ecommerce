using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IPerformAmazonUpdates
    {
        int Order { get; }
        bool Update(AmazonOrder amazonOrder, Order order);
    }
}