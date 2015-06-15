using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetProductSpecifications : GetUniversalSearchItemBase<ProductSpecificationAttribute>
    {
        public override UniversalSearchItem GetSearchItem(ProductSpecificationAttribute productSpecification)
        {
            return new UniversalSearchItem
            {
                DisplayName = productSpecification.Name,
                Id = productSpecification.Id,
                PrimarySearchTerms = new string[] { productSpecification.Name },
                SecondarySearchTerms = new string[] { productSpecification.Id.ToString() },
                SystemType = productSpecification.GetType().FullName,
                ActionUrl = string.Format("/admin/apps/ecommerce/productspecificationattribute/edit/{0}", productSpecification.Id),
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<ProductSpecificationAttribute> entities)
        {
            return entities.Select(GetSearchItem).ToHashSet();
        }
    }
}