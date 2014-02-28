using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Indexing.AmazonOrderSearch
{
    public class AmazonOrderEmailFieldDefinition : StringFieldDefinition<AmazonOrderSearchDefinition, AmazonOrder>
    {
        public AmazonOrderEmailFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "email", index: Field.Index.ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(AmazonOrder obj)
        {
            yield return obj.BuyerEmail;
        }
    }
}