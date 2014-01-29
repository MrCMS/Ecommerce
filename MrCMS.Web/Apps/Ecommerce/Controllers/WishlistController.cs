using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class WishlistController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IWishlistUIService _wishlistUIService;
        private readonly CartModel _cartModel;

        public WishlistController(IWishlistUIService wishlistUIService, CartModel cartModel)
        {
            _wishlistUIService = wishlistUIService;
            _cartModel = cartModel;
        }

        public ViewResult Show(Wishlist page)
        {
            ViewData["wishlist"] = _wishlistUIService.GetWishlist();
            ViewData["cart"] = _cartModel;
           return View(page);
        }

        public PartialViewResult Add(ProductVariant productVariant)
        {
            ViewData["in-wishlist"] = _wishlistUIService.IsInWishlist(productVariant);
            return PartialView(productVariant);
        }

        [HttpPost]
        [ActionName("Add")]
        public JsonResult Add_POST(ProductVariant productVariant)
        {
            _wishlistUIService.Add(productVariant);
            return Json(true);
        }

        [HttpPost]
        [ActionName("Remove")]
        public JsonResult Remove(ProductVariant productVariant)
        {
            _wishlistUIService.Remove(productVariant);
            return Json(true);
        }

        public PartialViewResult Summary()
        {
            return PartialView(_wishlistUIService.GetSummary());
        }
    }

    public interface IWishlistUIService
    {
        void Add(ProductVariant productVariant);
        WishlistSummary GetSummary();
        bool IsInWishlist(ProductVariant productVariant);
        void Remove(ProductVariant productVariant);
        IList<WishlistItem> GetWishlist();
    }

    public class WishlistSummary
    {
        public int Count { get; set; }
    }

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
                _session.Transact(session => session.Save(new WishlistItem
                                                              {
                                                                  Item = productVariant,
                                                                  UserGuid = _getUserGuid.UserGuid
                                                              }));
            }
        }

        public WishlistSummary GetSummary()
        {
            return new WishlistSummary
                       {
                           Count = GetCurrentUsersItemsQuery().RowCount()
                       };
        }

        public bool IsInWishlist(ProductVariant productVariant)
        {
            return GetCurrentUsersItemsQuery().Where(item => item.Item == productVariant).Any();
        }

        public void Remove(ProductVariant productVariant)
        {
            var wishlistItem = GetCurrentUsersItemsQuery().Where(item => item.Item == productVariant).SingleOrDefault();
            if (wishlistItem != null)
                _session.Transact(session => session.Delete(wishlistItem));
        }

        public IList<WishlistItem> GetWishlist()
        {
            return GetCurrentUsersItemsQuery().List();
        }

        private IQueryOver<WishlistItem, WishlistItem> GetCurrentUsersItemsQuery()
        {
            return _session.QueryOver<WishlistItem>().Where(item => item.UserGuid == _getUserGuid.UserGuid);
        }
    }
}