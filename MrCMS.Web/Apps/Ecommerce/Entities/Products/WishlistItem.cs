using System;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class WishlistItem : SiteEntity
    {
        public virtual Guid UserGuid { get; set; }
        public virtual ProductVariant Item { get; set; }
    }
}