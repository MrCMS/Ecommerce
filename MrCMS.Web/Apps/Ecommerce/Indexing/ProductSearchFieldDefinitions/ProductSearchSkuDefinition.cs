using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchSkuDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly ISession _session;

        public ProductSearchSkuDefinition(ILuceneSettingsService luceneSettingsService, ISession session)
            : base(luceneSettingsService, "skus", index: Field.Index.ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return GetSKUs(obj.Variants);
        }

        private static IEnumerable<string> GetSKUs(IEnumerable<ProductVariant> productVariants)
        {
            return productVariants.Select(x => x.SKU);
        }

        protected override Dictionary<Product, IEnumerable<string>> GetValues(List<Product> objs)
        {
            SkuList list = null;
            var skuLists = _session.QueryOver<ProductVariant>()
                .SelectList(builder => builder
                    .Select(variant => variant.SKU).WithAlias(() => list.SKU)
                    .Select(variant => variant.Product.Id).WithAlias(() => list.ProductId)
                )
                .TransformUsing(Transformers.AliasToBean<SkuList>())
                .List<SkuList>();
            var groupedSkus = skuLists.GroupBy(skuList => skuList.ProductId)
                .ToDictionary(lists => lists.Key, lists => lists.Select(skuList => skuList.SKU));

            return objs.ToDictionary(product => product,
                product => groupedSkus.ContainsKey(product.Id) ? groupedSkus[product.Id] : Enumerable.Empty<string>());
        }

        public class SkuList
        {
            public string SKU { get; set; }
            public int ProductId { get; set; }
        }
    }
}