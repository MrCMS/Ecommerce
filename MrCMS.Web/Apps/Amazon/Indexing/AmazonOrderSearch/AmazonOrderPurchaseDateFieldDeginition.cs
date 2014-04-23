using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Indexing.AmazonOrderSearch
{
    public class AmazonOrderPurchaseDateFieldDeginition : StringFieldDefinition<AmazonOrderSearchDefinition, AmazonOrder>
    {
        public AmazonOrderPurchaseDateFieldDeginition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "purcashdate", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(AmazonOrder obj)
        {
            yield return DateTools.DateToString(obj.PurchaseDate.GetValueOrDefault(DateTime.MaxValue), DateTools.Resolution.SECOND);
        }
    }
}