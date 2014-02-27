using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchPriceDefinition : DecimalFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchPriceDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "price", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<decimal> GetValues(Product obj)
        {
            return GetPrices(obj);
        }

        public IEnumerable<decimal> GetPrices(Product entity)
        {
            return entity.Variants.Select(pv => pv.Price);
        }
    }
}