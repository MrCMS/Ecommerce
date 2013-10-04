using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IShipAmazonOrderService
    {
        List<AmazonOrder> MarkOrdersAsShipped();
    }
}