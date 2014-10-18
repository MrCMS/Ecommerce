using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using UrlHelper = System.Web.Mvc.UrlHelper;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetDiscountCodeSearchItem : GetUniversalSearchItemBase<Discount>
    {
        private readonly UrlHelper _urlHelper;

        public GetDiscountCodeSearchItem(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override UniversalSearchItem GetSearchItem(Discount discount)
        {
            return new UniversalSearchItem
            {
                DisplayName = discount.Name,
                Id = discount.Id,
                SearchTerms = new string[] { discount.Id.ToString(), discount.Name, discount.Code },
                SystemType = discount.GetType().FullName,
                ActionUrl = _urlHelper.Action("Edit", "Discount", new { id = discount.Id, area = "admin" }),
            };
        }
    }
}