using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IImportAmazonOrderService
    {
        AmazonOrder SetAmazonOrderItems(IEnumerable<OrderItem> rawOrderItems, AmazonOrder amazonOrder);
        bool IsCurrencyValid(Order order);
    }
}