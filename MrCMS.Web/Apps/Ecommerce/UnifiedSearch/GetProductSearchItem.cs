using System.Collections.Generic;
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
            var searchTerms = new List<string> { product.Name, GetVariantNames(product) };
            searchTerms.AddRange(GetSkus(product));

            return new UniversalSearchItem
            {
                DisplayName = product.Name,
                Id = product.Id,
                SearchTerms = searchTerms,
                SystemType = product.GetType().FullName,
                ActionUrl = _urlHelper.Action("Edit", "Webpage", new { id = product.Id, area = "admin" }),
            };
        }

        private string GetVariantNames(Product product)
        {
            var productVariants = product.Variants;
            var sb = new StringBuilder();
            foreach (var productVariant in productVariants.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                sb.Append(productVariant.Name + ",");
            }
            return sb.ToString();
        }

        private IEnumerable<string> GetSkus(Product product)
        {
            var productVariants = product.Variants;
            var sb = new StringBuilder();
            foreach (var productVariant in productVariants)
            {
                yield return productVariant.SKU;
                yield return productVariant.ManufacturerPartNumber;
            }
        }
    }
}