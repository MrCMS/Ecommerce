using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchBodyContentDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchBodyContentDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "bodycontent", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return obj.BodyContent;
        }
    }
}