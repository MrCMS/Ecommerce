using MrCMS.Entities.Documents.Media;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using UrlHelper = System.Web.Mvc.UrlHelper;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetOrderSearchItem : GetUniversalSearchItemBase<Order>
    {
        private readonly UrlHelper _urlHelper;

        public GetOrderSearchItem(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

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
                SearchTerms = new string[] { order.Id.ToString(), billingAddress.GetDescription(), order.ShippingAddress.GetDescription() },
                SystemType = order.GetType().FullName,
                ActionUrl = _urlHelper.Action("Edit", "Order", new { id = order.Id, area = "admin" }),
            };
        }
    }
}