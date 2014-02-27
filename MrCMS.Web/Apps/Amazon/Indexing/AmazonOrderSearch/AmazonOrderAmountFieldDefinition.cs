using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Indexing.AmazonOrderSearch
{
    public class AmazonOrderAmountFieldDefinition : DecimalFieldDefinition<AmazonOrderSearchDefinition, AmazonOrder>
    {
        public AmazonOrderAmountFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "orderamount", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<decimal> GetValues(AmazonOrder obj)
        {
            yield return obj.OrderTotalAmount;
        }
    }
}