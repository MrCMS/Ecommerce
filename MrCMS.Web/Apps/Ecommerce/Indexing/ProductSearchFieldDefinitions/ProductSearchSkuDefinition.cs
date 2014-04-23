using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchSkuDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchSkuDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "skus", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return GetSKUs(obj.Variants);
        }

        private static IEnumerable<string> GetSKUs(IEnumerable<ProductVariant> productVariants)
        {
            return productVariants.Select(x => x.SKU);
        }
    }
}