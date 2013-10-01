using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Orders
{
    public interface IAmazonOrdersApiService
    {
        AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection);
        IEnumerable<Order> ListOrders(AmazonSyncModel model);
        IEnumerable<Order> GetOrder(AmazonSyncModel model);
        IEnumerable<OrderItem> ListOrderItems(string amazonOrderId);
    }
}