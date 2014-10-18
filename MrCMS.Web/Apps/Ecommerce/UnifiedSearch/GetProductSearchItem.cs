using System.Linq;
using System.Text;
using System.Web.Mvc;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetProductSearchItem : GetUniversalSearchItemBase<Product>
    {
        private readonly UrlHelper _urlHelper;

        public GetProductSearchItem(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override UniversalSearchItem GetSearchItem(Product product)
        {
            return new UniversalSearchItem
            {
                DisplayName = product.Name,
                Id = product.Id,
                SearchTerms = new string[] { product.Name, GetSkus(product), GetVariantNames(product) },
                SystemType = product.GetType().FullName,
                ActionUrl = _urlHelper.Action("Edit", "Webpage", new { id = product.Id, area = "admin" }),
            };
        }

        private string GetVariantNames(Product product)
        {
            var productVariants = product.Variants;
            var sb = new StringBuilder();
            foreach (var productVariant in productVariants.Where(x=>!string.IsNullOrWhiteSpace(x.Name)))
            {
                sb.Append(productVariant.Name);
            }
            return sb.ToString();
        }

        private string GetSkus(Product product)
        {
            var productVariants = product.Variants;
            var sb = new StringBuilder();
            foreach (var productVariant in productVariants)
            {
                sb.Append(productVariant.SKU);
                sb.Append(productVariant.ManufacturerPartNumber);
            }
            return sb.ToString();
        }
    }
}