using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Orders
{
    public interface IAmazonOrdersApiService
    {
        bool IsLive(AmazonApiSection apiSection);
        IEnumerable<Order> ListOrders(AmazonSyncModel model);
        IEnumerable<OrderItem> ListOrderItems(string amazonOrderId);
        List<Order> ListCreatedOrders(GetUpdatedOrdersRequest updatedOrdersRequest);
        List<Order> ListUpdatedOrders(GetUpdatedOrdersRequest updatedOrdersRequest);
    }
}