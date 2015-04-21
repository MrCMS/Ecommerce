using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using UrlHelper = System.Web.Mvc.UrlHelper;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetDiscountCodeSearchItem : GetUniversalSearchItemBase<Discount>
    {
        public override UniversalSearchItem GetSearchItem(Discount discount)
        {
            return new UniversalSearchItem
            {
                DisplayName = discount.Name,
                Id = discount.Id,
                PrimarySearchTerms = new string[] { discount.Name, discount.Code },
                SecondarySearchTerms = new[] { discount.Id.ToString(), },
                SystemType = typeof(Discount).FullName,
                ActionUrl = string.Format("/admin/apps/ecommerce/discount/edit/{0}", discount.Id),
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Discount> entities)
        {
            return entities.Select(GetSearchItem).ToHashSet();
        }
    }
}