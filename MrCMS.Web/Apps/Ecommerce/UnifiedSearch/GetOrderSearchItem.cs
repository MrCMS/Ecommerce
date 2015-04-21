using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using UrlHelper = System.Web.Mvc.UrlHelper;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetOrderSearchItem : GetUniversalSearchItemBase<Order>
    {
        public override UniversalSearchItem GetSearchItem(Order order)
        {
            AddressData billingAddress = order.BillingAddress;
            var data = " - " + "No Name";
            if (billingAddress != null)
                data = " - " + billingAddress.Name;
            return new UniversalSearchItem
            {
                DisplayName = "#" + order.Id + data,
                Id = order.Id,
                PrimarySearchTerms = new[] { billingAddress.GetDescription(), order.ShippingAddress.GetDescription() },
                SecondarySearchTerms = new[] { order.Id.ToString() },
                SystemType = order.GetType().FullName,
                ActionUrl = string.Format("/admin/apps/ecommerce/order/edit/{0}", order.Id),
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Order> entities)
        {
            return entities.Select(GetSearchItem).ToHashSet();
        }
    }
}