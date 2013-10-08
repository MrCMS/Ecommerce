using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Orders
{
    public interface IAmazonOrdersApiService
    {
        List<Order> ListSpecificOrders(IEnumerable<string> orderIds);

        List<Order> ListCreatedOrders(GetUpdatedOrdersRequest updatedOrdersRequest);
        IEnumerable<Order> ListUpdatedOrders(GetUpdatedOrdersRequest updatedOrdersRequest);

        IEnumerable<OrderItem> ListOrderItems(string amazonOrderId);
    }
}