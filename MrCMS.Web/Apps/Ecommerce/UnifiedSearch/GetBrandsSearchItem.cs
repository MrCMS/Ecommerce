using System.Web.Mvc;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetBrandsSearchItem : GetUniversalSearchItemBase<Brand>
    {
        private readonly UrlHelper _urlHelper;

        public GetBrandsSearchItem(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override UniversalSearchItem GetSearchItem(Brand brand)
        {
            return new UniversalSearchItem
            {
                DisplayName = brand.Name,
                Id = brand.Id,
                SearchTerms = new string[] { brand.Id.ToString(), brand.Name },
                SystemType = brand.GetType().FullName,
                ActionUrl = _urlHelper.Action("Edit", "Brand", new { id = brand.Id, area = "admin" }),
            };
        }
    }
}