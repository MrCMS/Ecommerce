using System;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Wishlists
{
    public class WishlistUIService : IWishlistUIService
    {
        private readonly IGetUserGuid _getUserGuid;
        private readonly ISession _session;

        public WishlistUIService(IGetUserGuid getUserGuid, ISession session)
        {
            _getUserGuid = getUserGuid;
            _session = session;
        }

        public void Add(ProductVariant productVariant)
        {
            if (!IsInWishlist(productVariant))
            {
                var wishlist = GetMyWishlist(true);
                var wishlistItem = new WishlistItem
                                       {
                                           Item = productVariant,
                                           Wishlist = wishlist,
                                       };
                wishlist.WishlistItems.Add(wishlistItem);
                _session.Transact(session => session.Save(wishlistItem));
            }
        }

        public WishlistSummary GetSummary()
        {
            var wishlist = GetMyWishlist();
            return new WishlistSummary
                       {
                           Count = wishlist == null ? 0 : wishlist.ItemCount,
                       };
        }

        public bool IsInWishlist(ProductVariant productVariant)
        {
            var myWishlist = GetMyWishlist();
            if (myWishlist == null)
                return false;
            return myWishlist.WishlistItems.Any(item => item.Item == productVariant);
        }

        public void Remove(ProductVariant productVariant)
        {
            var wishlist = GetMyWishlist(true);
            var wishlistItem = wishlist.WishlistItems.SingleOrDefault(item => item.Item == productVariant);
            if (wishlistItem != null)
            {
                wishlist.WishlistItems.Remove(wishlistItem);
                _session.Transact(session => session.Delete(wishlistItem));
            }
        }

        public Wishlist GetWishlist(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return GetMyWishlist(true);
            Guid result;
            if (Guid.TryParse(guid, out result))
                return _session.QueryOver<Wishlist>()
                               .Where(wishlist => wishlist.Guid == result)
                               .SingleOrDefault();
            return null;
        }

        private Wishlist GetMyWishlist(bool createIfDoesNotExist = false)
        {
            return _session.QueryOver<Wishlist>()
                           .Where(wishlist => wishlist.UserGuid == _getUserGuid.UserGuid)
                           .SingleOrDefault()
                   ?? (createIfDoesNotExist ? CreateWishlist() : null);
        }

        private Wishlist CreateWishlist()
        {
            var wishlist = new Wishlist
                               {
                                   UserGuid = _getUserGuid.UserGuid,
                               };
            _session.Transact(session => session.Save(wishlist));
            return wishlist;
        }
    }
}