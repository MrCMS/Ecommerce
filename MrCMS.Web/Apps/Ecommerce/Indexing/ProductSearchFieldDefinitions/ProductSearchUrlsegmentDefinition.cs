using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchUrlsegmentDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchUrlsegmentDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "urlsegment", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return obj.UrlSegment;
        }
    }
}