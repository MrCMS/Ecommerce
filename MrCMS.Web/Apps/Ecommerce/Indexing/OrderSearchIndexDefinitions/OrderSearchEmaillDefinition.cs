using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions
{
    public class OrderSearchEmaillDefinition : StringFieldDefinition<OrderSearchIndex, Order>
    {
        public OrderSearchEmaillDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "orderemail", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Order obj)
        {
            yield return obj.OrderEmail;
        }
    }
}