using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetBrandsSearchItem : GetUniversalSearchItemBase<Brand>
    {
        public override UniversalSearchItem GetSearchItem(Brand brand)
        {
            return new UniversalSearchItem
            {
                DisplayName = brand.Name,
                Id = brand.Id,
                PrimarySearchTerms = new string[] { brand.Name },
                SecondarySearchTerms = new string[] { brand.Id.ToString() },
                SystemType = typeof(Brand).FullName,
                ActionUrl = "/admin/apps/ecommerce/brand/edit/" + brand.Id,
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Brand> entities)
        {
            return entities.Select(GetSearchItem).ToHashSet();
        }
    }
}