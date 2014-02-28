using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchSpecificationsDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchSpecificationsDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "specifications", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return
                obj.SpecificationValues.Select(value => value.ProductSpecificationAttributeOption.Id.ToString())
                   .Distinct();
        }
    }
}