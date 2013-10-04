using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IUpdateAmazonOrder
    {
        AmazonOrder UpdateOrder(Order order);
    }
}