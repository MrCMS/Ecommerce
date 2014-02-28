using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Indexing.AmazonOrderSearch
{
    public class AmazonOrderStatusFieldDefinition : StringFieldDefinition<AmazonOrderSearchDefinition, AmazonOrder>
    {
        public AmazonOrderStatusFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "status", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(AmazonOrder obj)
        {
            yield return obj.Status.ToString();
        }
    }
}