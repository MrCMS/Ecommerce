using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class ProductAddedToCartMetadata : DocumentMetadataMap<ProductAddedToCart>
    {
        public override string WebGetController
        {
            get { return "ProductAddedToCart"; }
        }

        public override string WebGetAction
        {
            get { return "Show"; }
        }

        public override string App
        {
            get { return "Ecommerce"; }
        }
    }
}