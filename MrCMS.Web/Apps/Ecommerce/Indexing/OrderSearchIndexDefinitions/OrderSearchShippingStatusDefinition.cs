using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions
{
    public class OrderSearchShippingStatusDefinition : StringFieldDefinition<OrderSearchIndex, Order>
    {
        public OrderSearchShippingStatusDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "shippingstatus", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Order obj)
        {
            yield return obj.ShippingStatus.ToString();
        }
    }
}