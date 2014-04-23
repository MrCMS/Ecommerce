using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Indexing.AmazonOrderSearch
{
    public class AmazonOrderNameFieldDefinition : StringFieldDefinition<AmazonOrderSearchDefinition, AmazonOrder>
    {
        public AmazonOrderNameFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "name", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(AmazonOrder obj)
        {
            yield return obj.BuyerName;
        }
    }
}