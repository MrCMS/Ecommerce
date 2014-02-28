using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions
{
    public class OrderSearchLastnamelDefinition : StringFieldDefinition<OrderSearchIndex, Order>
    {
        public OrderSearchLastnamelDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "lastname", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Order obj)
        {
            yield return obj.BillingAddress != null ? obj.BillingAddress.Name : String.Empty;
        }
    }
}