using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class WishlistMetadata : DocumentMetadataMap<Wishlist>
    {
        public override string WebGetController
        {
            get { return "Wishlist"; }
        }
    }
}