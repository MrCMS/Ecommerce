using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class ShowWishlistMetadata : DocumentMetadataMap<ShowWishlist>
    {
        public override string WebGetController
        {
            get { return "Wishlist"; }
        }
    }
}