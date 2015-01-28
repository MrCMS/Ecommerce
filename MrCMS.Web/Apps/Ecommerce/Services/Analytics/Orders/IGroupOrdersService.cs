using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders
{
    public interface IGroupOrdersService
    {
        IList<KeyValuePair<string, decimal>> GetOrdersGroupedByShipmentType(IEnumerable<IGrouping<string, Order>> baseData, string salesChannel);
    }
}