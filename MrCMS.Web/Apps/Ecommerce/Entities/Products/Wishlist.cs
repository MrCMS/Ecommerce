using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class Wishlist : SiteEntity
    {
        public Wishlist()
        {
            WishlistItems = new List<WishlistItem>();
            Guid = Guid.NewGuid();
        }

        public virtual Guid Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid UserGuid { get; set; }
        public virtual IList<WishlistItem> WishlistItems { get; set; }

        public virtual int ItemCount
        {
            get { return WishlistItems.Count; }
        }
    }
}