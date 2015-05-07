using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.CustomerFeedback.Pages;

namespace MrCMS.Web.Apps.CustomerFeedback.Metadata
{
    public class OrderFeedbackMetadata : DocumentMetadataMap<OrderFeedback>
    {
        public override string WebGetController
        {
            get { return "OrderFeedback"; }
        }

        public override bool RevealInNavigation
        {
            get { return false; }
        }
    }
}