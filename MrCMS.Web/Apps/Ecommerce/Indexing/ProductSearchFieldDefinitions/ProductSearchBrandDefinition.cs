using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchBrandDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchBrandDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "brands", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            if (obj.Brand != null)
                yield return obj.Brand.Id.ToString();

            yield return null;
        }
    }
}