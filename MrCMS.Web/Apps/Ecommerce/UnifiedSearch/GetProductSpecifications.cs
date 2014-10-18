using System.Web.Mvc;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetProductSpecifications : GetUniversalSearchItemBase<ProductSpecificationAttribute>
    {
        private readonly UrlHelper _urlHelper;

        public GetProductSpecifications(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override UniversalSearchItem GetSearchItem(ProductSpecificationAttribute productSpecification)
        {
            return new UniversalSearchItem
            {
                DisplayName = productSpecification.Name,
                Id = productSpecification.Id,
                SearchTerms = new string[] { productSpecification.Id.ToString(), productSpecification.Name },
                SystemType = productSpecification.GetType().FullName,
                ActionUrl = _urlHelper.Action("Edit", "ProductSpecificationAttribute", new { id = productSpecification.Id, area = "admin" }),
            };
        }
    }
}