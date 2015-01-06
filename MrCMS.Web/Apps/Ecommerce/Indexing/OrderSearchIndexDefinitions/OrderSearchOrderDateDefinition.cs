using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions
{
    public class OrderSearchOrderDateDefinition : StringFieldDefinition<OrderSearchIndex, Order>
    {
        public OrderSearchOrderDateDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "order-date", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Order obj)
        {
            yield return DateTools.DateToString(obj.OrderDate.GetValueOrDefault(obj.CreatedOn), DateTools.Resolution.SECOND);
        }
    }
}