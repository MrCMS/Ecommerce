using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ryness.Pages;

namespace MrCMS.Web.Apps.Ryness.Metadata
{
    public class BuyingGuideMetadata : DocumentMetadataMap<BuyingGuide>
    {
        public override string DefaultLayoutName
        {
            get { return "Content Page Layout"; }
        }
    }
}