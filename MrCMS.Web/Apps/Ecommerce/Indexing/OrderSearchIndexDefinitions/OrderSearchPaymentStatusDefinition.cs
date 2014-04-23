using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions
{
    public class OrderSearchPaymentStatusDefinition : StringFieldDefinition<OrderSearchIndex, Order>
    {
        public OrderSearchPaymentStatusDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "paymentstatus", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Order obj)
        {
            yield return obj.PaymentStatus.ToString();
        }
    }
}