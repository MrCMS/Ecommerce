using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartImpl : IGetCart
    {
        private readonly ISession _session;

        public GetCartImpl(ISession session)
        {
            _session = session;
        }

        public CartModel GetCart()
        {
            return new CartModel
                       {
                           User = CurrentRequestData.CurrentUser,
                           UserGuid = CurrentRequestData.UserGuid,
                           Items = GetItems(),
                           ShippingAddress = GetShippingAddress(),
                           ShippingMethod = GetShippingMethod()
                       };
        }

        private List<CartItem> GetItems()
        {
            return
                _session.QueryOver<CartItem>()
                        .Where(item => item.UserGuid == CurrentRequestData.UserGuid)
                        .Cacheable()
                        .List().ToList();
        }

        private Address GetShippingAddress()
        {
            return CurrentRequestData.CurrentContext.Session != null
                       ? CurrentRequestData.CurrentContext.Session["current.shipping-address"] as Address
                       : null;
        }

        private ShippingMethod GetShippingMethod()
        {
            var shippingMethodId = CurrentRequestData.CurrentContext.Session != null
                                       ? CurrentRequestData.CurrentContext.Session["current.shipping-method-id"] as int?
                                       : null;

            return shippingMethodId.HasValue
                       ? _session.Get<ShippingMethod>(shippingMethodId.Value)
                       : null;
        }
    }
}