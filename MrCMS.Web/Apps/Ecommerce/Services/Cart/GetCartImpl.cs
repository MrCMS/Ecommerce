using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartImpl : IGetCart
    {
        private readonly ISession _session;
        private readonly CurrentSite _currentSite;

        public GetCartImpl(ISession session, CurrentSite currentSite)
        {
            _session = session;
             _currentSite = currentSite;
        }

        public CartModel GetCart()
        {
            return new CartModel
                       {
                           User = CurrentRequestData.CurrentUser,
                           UserGuid = CurrentRequestData.UserGuid,
                           Items = GetItems(),
                           ShippingAddress = GetShippingAddress(),
                           BillingAddress = GetBillingAddress(),
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

        private Address GetBillingAddress()
        {
            return CurrentRequestData.CurrentContext.Session != null
                       ? CurrentRequestData.CurrentContext.Session["current.billing-address"] as Address
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

        public MrCMS.Web.Apps.Ecommerce.Pages.Cart GetSiteCart()
        {
            IList<MrCMS.Web.Apps.Ecommerce.Pages.Cart> carts = _session.QueryOver<MrCMS.Web.Apps.Ecommerce.Pages.Cart>().Where(x => x.Site == _currentSite.Site).Cacheable().List();
            if (carts.Any())
                return _session.QueryOver<MrCMS.Web.Apps.Ecommerce.Pages.Cart>().Cacheable().List().First();
            else
                return null;
        }
    }
}