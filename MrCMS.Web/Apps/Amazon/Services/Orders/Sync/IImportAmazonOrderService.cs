using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IImportAmazonOrderService
    {
        void ImportOrders(AmazonSyncModel model, ICollection<AmazonOrder> orders);
        List<AmazonOrder> GetOrdersFromAmazon(AmazonSyncModel model);
    }
}