using System;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services.Wishlists
{
    public interface IWishlistUIService
    {
        void Add(ProductVariant productVariant);
        WishlistSummary GetSummary();
        bool IsInWishlist(ProductVariant productVariant);
        void Remove(ProductVariant productVariant);
        Wishlist GetWishlist(string guid);
        CanBuyStatus CanBuy(ProductVariant productVariant);
    }
}