using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchMetaKeywordsDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchMetaKeywordsDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metakeywords", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return obj.MetaKeywords;
        }
    }
}