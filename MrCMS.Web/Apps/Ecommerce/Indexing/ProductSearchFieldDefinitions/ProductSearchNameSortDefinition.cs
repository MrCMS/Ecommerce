using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchNameSortDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchNameSortDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "nameSort", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return obj.Name.Trim().ToLower();
        }
    }
}