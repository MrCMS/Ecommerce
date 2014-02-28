using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchCreatedOnDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchCreatedOnDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "createdon", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return DateTools.DateToString(obj.CreatedOn, DateTools.Resolution.SECOND);
        }
    }
}