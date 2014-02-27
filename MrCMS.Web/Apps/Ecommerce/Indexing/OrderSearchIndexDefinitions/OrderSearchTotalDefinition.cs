using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions
{
    public class OrderSearchTotalDefinition : DecimalFieldDefinition<OrderSearchIndex, Order>
    {
        public OrderSearchTotalDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "total", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<decimal> GetValues(Order obj)
        {
            yield return obj.Total;
        }
    }
}