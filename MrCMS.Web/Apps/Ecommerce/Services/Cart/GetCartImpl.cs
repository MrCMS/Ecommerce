using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using System;
using NHibernate.Criterion;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

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
            var address = GetShippingAddress() ?? new Address();
            if (GetCountry()!=null)
                address.Country = GetCountry();
            var cart=new CartModel
                       {
                           User = CurrentRequestData.CurrentUser,
                           UserGuid = CurrentRequestData.UserGuid,
                           Items = GetItems(),
                           ShippingAddress = address,
                           BillingAddress = GetBillingAddress(),
                           ShippingMethod = GetShippingMethod(),
                           OrderEmail = GetOrderEmail(),
                           DiscountCode = GetDiscountCode(),
                           Discount = GetDiscount()
                       };
            return cart;
        }

        private decimal? GetShippingTotal()
        {
            if (GetItems().Any())
                return 0;
            return GetItems().Sum(x => x.Price);
        }

        private List<CartItem> GetItems()
        {
            return
                _session.QueryOver<CartItem>()
                        .Where(item => item.UserGuid == CurrentRequestData.UserGuid)
                        .Cacheable()
                        .List().ToList();
        }

        private Discount GetDiscount()
        {
            if (!String.IsNullOrWhiteSpace(GetDiscountCode()))
                return
                    _session.QueryOver<Discount>()
                            .Where(item => item.Code.IsInsensitiveLike(GetDiscountCode(), MatchMode.Exact))
                            .Cacheable()
                            .SingleOrDefault();
            else
                return null;
        }

        public Address GetShippingAddress()
        {
            return CurrentRequestData.CurrentContext.Session != null
                       ? CurrentRequestData.CurrentContext.Session["current.shipping-address"] as Address
                       : null;
        }

        public void SetShippingAddress(Address address)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session["current.shipping-address"] != null)
                CurrentRequestData.CurrentContext.Session["current.shipping-address"] = address;
            else
                CurrentRequestData.CurrentContext.Session.Add("current.shipping-address", address);
        }

        public Address GetBillingAddress()
        {
            return CurrentRequestData.CurrentContext.Session != null
                       ? CurrentRequestData.CurrentContext.Session["current.billing-address"] as Address
                       : null;
        }

        public void SetBillingAddress(Address address)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session["current.billing-address"] != null)
                CurrentRequestData.CurrentContext.Session["current.billing-address"] = address;
            else
                CurrentRequestData.CurrentContext.Session.Add("current.billing-address", address);
        }

        public ShippingMethod GetShippingMethod()
        {
            var shippingMethodId = CurrentRequestData.CurrentContext.Session != null
                                       ? CurrentRequestData.CurrentContext.Session["current.shipping-method-id"] as int?
                                       : null;

            return shippingMethodId.HasValue
                       ? _session.Get<ShippingMethod>(shippingMethodId.Value)
                       : null;
        }

        public void SetShippingMethod(int shippingMethodId)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session["current.shipping-method-id"] != null)
                CurrentRequestData.CurrentContext.Session["current.shipping-method-id"] = shippingMethodId;
            else
                CurrentRequestData.CurrentContext.Session.Add("current.shipping-method-id", shippingMethodId);
        }

        public void SetOrderEmail(string value)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session["current.order-email"] != null)
                CurrentRequestData.CurrentContext.Session["current.order-email"] = value;
            else
                CurrentRequestData.CurrentContext.Session.Add("current.order-email", value);
        }
        public string GetOrderEmail()
        {
            return CurrentRequestData.CurrentContext.Session != null
                           ? CurrentRequestData.CurrentContext.Session["current.order-email"] as string
                           : String.Empty;
        }
        public void SetDiscountCode(string value)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session["current.discount-code"] != null)
                CurrentRequestData.CurrentContext.Session["current.discount-code"] = value;
            else
                CurrentRequestData.CurrentContext.Session.Add("current.discount-code", value);
        }
        public string GetDiscountCode()
        {
            return CurrentRequestData.CurrentContext.Session != null
                           ? CurrentRequestData.CurrentContext.Session["current.discount-code"] as string
                           : String.Empty;
        }
        public void SetPaymentMethod(string value)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session["current.payment-method"] != null)
                CurrentRequestData.CurrentContext.Session["current.payment-method"] = value;
            else
                CurrentRequestData.CurrentContext.Session.Add("current.payment-method", value);
        }
        public string GetPaymentMethod()
        {
            return CurrentRequestData.CurrentContext.Session != null
                           ? CurrentRequestData.CurrentContext.Session["current.payment-method"] as string
                           : String.Empty;
        }

        public void SetCountry(int id)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session["current.country-id"] != null)
                CurrentRequestData.CurrentContext.Session["current.country-id"] = id;
            else
                CurrentRequestData.CurrentContext.Session.Add("current.country-id", id);
        }
        public Country GetCountry()
        {
            var id = CurrentRequestData.CurrentContext.Session != null
                                        ? CurrentRequestData.CurrentContext.Session["current.country-id"] as int?
                                        : null;

            return id.HasValue
                       ? _session.Get<Country>(id.Value)
                       : null;
        }
    }
}