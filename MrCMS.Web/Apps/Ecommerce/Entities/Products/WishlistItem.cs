using System;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class WishlistItem : SiteEntity
    {
        public virtual Wishlist Wishlist { get; set; }
        public virtual ProductVariant Item { get; set; }

    }
}