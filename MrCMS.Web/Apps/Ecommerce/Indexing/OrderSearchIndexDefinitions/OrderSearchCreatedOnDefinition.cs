using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions
{
    public class OrderSearchCreatedOnDefinition : StringFieldDefinition<OrderSearchIndex, Order>
    {
        public OrderSearchCreatedOnDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "createdon", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Order obj)
        {
            yield return DateTools.DateToString(obj.CreatedOn, DateTools.Resolution.SECOND);
        }
    }
}