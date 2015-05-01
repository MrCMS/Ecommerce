using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.CustomerFeedback.Pages;

namespace MrCMS.Web.Apps.CustomerFeedback.Metadata
{
    public class CustomerInteractionMetadata : DocumentMetadataMap<CustomerInteraction>
    {
        public override string WebGetController
        {
            get { return "CustomerInteraction"; }
        }
    }
}