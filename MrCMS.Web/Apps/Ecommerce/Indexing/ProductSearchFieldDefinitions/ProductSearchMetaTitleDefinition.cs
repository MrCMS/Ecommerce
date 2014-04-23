using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchMetaTitleDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchMetaTitleDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metatitle", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return obj.MetaTitle;
        }
    }
}