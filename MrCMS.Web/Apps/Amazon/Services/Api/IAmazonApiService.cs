using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public interface IAmazonApiService
    {
        ServiceStatusEnum GetServiceStatus(AmazonApiSection apiSection);
        IEnumerable<Order> ListOrders(AmazonSyncModel model);
        IEnumerable<OrderItem> ListOrderItems(AmazonSyncModel model, string amazonOrderId);
    }
}