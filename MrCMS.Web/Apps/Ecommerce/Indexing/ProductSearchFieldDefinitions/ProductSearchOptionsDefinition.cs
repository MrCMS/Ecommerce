using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchOptionsDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchOptionsDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "options", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return
                obj.Variants.SelectMany(variant => variant.OptionValues.Select(value => value.Id))
                   .Select(i => i.ToString());
        }
    }
}