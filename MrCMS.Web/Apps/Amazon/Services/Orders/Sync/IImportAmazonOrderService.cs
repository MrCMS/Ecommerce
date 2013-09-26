using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IImportAmazonOrderService
    {
        AmazonOrder SetAmazonOrderItems(Order rawOrder,
                                        IEnumerable<OrderItem> rawOrderItems, AmazonOrder amazonOrder);
        AmazonOrder GetAmazonOrder(Order rawOrder);
    }
}