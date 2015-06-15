using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.UnifiedSearch
{
    public class GetProductSearchItem : GetUniversalSearchItemBase<Product>
    {
        private readonly ISession _session;

        public GetProductSearchItem( ISession session)
        {
            _session = session;
        }

        public override UniversalSearchItem GetSearchItem(Product product)
        {
            var productVariants = _session.QueryOver<ProductVariant>().Where(x => x.Product.Id == product.Id).Cacheable().List();
            return GetItem(product, productVariants);
        }

        private UniversalSearchItem GetItem(Product product, IList<ProductVariant> productVariants)
        {
            var searchTerms = new List<string> {product.Name, GetVariantNames(productVariants)};

            return new UniversalSearchItem
            {
                DisplayName = product.Name,
                Id = product.Id,
                PrimarySearchTerms = searchTerms,
                SecondarySearchTerms = GetSkus(productVariants),
                SystemType = product.GetType().FullName,
                ActionUrl = string.Format("/admin/webpage/edit/{0}", product.Id),
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Product> entities)
        {
            var allVariants =
                _session.QueryOver<ProductVariant>()
                    .Where(x => x.Product != null)
                    .Cacheable()
                    .List()
                    .GroupBy(x => x.Product.Id)
                    .ToDictionary(x => x.Key);

            return
                entities.Select(
                    product =>
                        GetItem(product,
                            allVariants.ContainsKey(product.Id)
                                ? allVariants[product.Id].ToList()
                                : new List<ProductVariant>())).ToHashSet();
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