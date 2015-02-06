using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetProductSearchItem : GetUniversalSearchItemBase<Product>
    {
        private readonly UrlHelper _urlHelper;
        private readonly ISession _session;

        public GetProductSearchItem(UrlHelper urlHelper, ISession session)
        {
            _urlHelper = urlHelper;
            _session = session;
        }

        public override UniversalSearchItem GetSearchItem(Product product)
        {
            var productVariants = _session.QueryOver<ProductVariant>().Where(x => x.Product.Id == product.Id).Cacheable().List();
            var searchTerms = new List<string> { product.Name, GetVariantNames(productVariants) };
            searchTerms.AddRange(GetSkus(productVariants));

            return new UniversalSearchItem
            {
                DisplayName = product.Name,
                Id = product.Id,
                SearchTerms = searchTerms,
                SystemType = product.GetType().FullName,
                ActionUrl = _urlHelper.Action("Edit", "Webpage", new { id = product.Id, area = "admin" }),
            };
        }

        private string GetVariantNames(IList<ProductVariant> productVariants)
        {
            var sb = new StringBuilder();
            foreach (var productVariant in productVariants.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                sb.Append(productVariant.Name + ",");
            }
            return sb.ToString();
        }

        private IEnumerable<string> GetSkus(IList<ProductVariant> productVariants)
        {
            foreach (var productVariant in productVariants)
            {
                yield return productVariant.SKU;
                yield return productVariant.ManufacturerPartNumber;
            }
        }
    }
}