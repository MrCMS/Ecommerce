using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchPublishOnDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchPublishOnDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "publishon", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return DateTools.DateToString(
                obj.PublishOn.GetValueOrDefault(DateTime.MaxValue),
                DateTools.Resolution.SECOND);
        }
    }
}