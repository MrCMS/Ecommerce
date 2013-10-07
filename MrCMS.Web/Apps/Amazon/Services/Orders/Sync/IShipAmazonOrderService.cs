using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IShipAmazonOrderService
    {
        List<AmazonOrder> MarkOrdersAsShipped();
    }
}