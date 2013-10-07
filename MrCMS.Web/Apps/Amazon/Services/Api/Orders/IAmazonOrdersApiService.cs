using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Orders
{
    public interface IAmazonOrdersApiService
    {
        bool IsLive(AmazonApiSection apiSection);
        
        List<Order> ListSpecificOrders(List<string> orderIds);

        List<Order> ListCreatedOrders(GetUpdatedOrdersRequest updatedOrdersRequest);
        IEnumerable<Order> ListUpdatedOrders(GetUpdatedOrdersRequest updatedOrdersRequest);

        IEnumerable<OrderItem> ListOrderItems(string amazonOrderId);
    }
}