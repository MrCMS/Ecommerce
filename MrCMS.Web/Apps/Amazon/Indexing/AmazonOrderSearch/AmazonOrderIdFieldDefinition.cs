using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Indexing.AmazonOrderSearch
{
    public class AmazonOrderIdFieldDefinition : StringFieldDefinition<AmazonOrderSearchDefinition, AmazonOrder>
    {
        public AmazonOrderIdFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "amazonorderid", index:Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(AmazonOrder obj)
        {
            yield return obj.AmazonOrderId;
        }
    }
}