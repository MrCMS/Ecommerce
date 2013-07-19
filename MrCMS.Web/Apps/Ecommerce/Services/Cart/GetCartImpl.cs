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
        private const string CurrentShippingAddressKey = "current.shipping-address";
        private const string CurrentBillingAddressKey = "current.billing-address";
        private const string CurrentShippingMethodIdKey = "current.shipping-method-id";
        private const string CurrentOrderEmailKey = "current.order-email";
        private const string CurrentDiscountCodeKey = "current.discount-code";
        private const string CurrentPaymentMethodKey = "current.payment-method";
        private const string CurrentCountryIdKey = "current.country-id";
        private readonly ISession _session;

        public GetCartImpl(ISession session)
        {
            _session = session;
        }

        public CartModel GetCart()
        {
            var address = GetShippingAddress() ?? new Address();
            if (GetCountry() != null)
                address.Country = GetCountry();
            var cart = new CartModel
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
                       ? CurrentRequestData.CurrentContext.Session[CurrentShippingAddressKey] as Address
                       : null;
        }

        public void SetShippingAddress(Address address)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session[CurrentShippingAddressKey] != null)
                CurrentRequestData.CurrentContext.Session[CurrentShippingAddressKey] = address;
            else
                CurrentRequestData.CurrentContext.Session.Add(CurrentShippingAddressKey, address);
        }

        public Address GetBillingAddress()
        {
            return CurrentRequestData.CurrentContext.Session != null
                       ? CurrentRequestData.CurrentContext.Session[CurrentBillingAddressKey] as Address
                       : null;
        }

        public void SetBillingAddress(Address address)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session[CurrentBillingAddressKey] != null)
                CurrentRequestData.CurrentContext.Session[CurrentBillingAddressKey] = address;
            else
                CurrentRequestData.CurrentContext.Session.Add(CurrentBillingAddressKey, address);
        }

        public ShippingMethod GetShippingMethod()
        {
            var shippingMethodId = CurrentRequestData.CurrentContext.Session != null
                                       ? CurrentRequestData.CurrentContext.Session[CurrentShippingMethodIdKey] as int?
                                       : null;

            return shippingMethodId.HasValue
                       ? _session.Get<ShippingMethod>(shippingMethodId.Value)
                       : null;
        }

        public void SetShippingMethod(int shippingMethodId)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session[CurrentShippingMethodIdKey] != null)
                CurrentRequestData.CurrentContext.Session[CurrentShippingMethodIdKey] = shippingMethodId;
            else
                CurrentRequestData.CurrentContext.Session.Add(CurrentShippingMethodIdKey, shippingMethodId);
        }

        public void SetOrderEmail(string value)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session[CurrentOrderEmailKey] != null)
                CurrentRequestData.CurrentContext.Session[CurrentOrderEmailKey] = value;
            else
                CurrentRequestData.CurrentContext.Session.Add(CurrentOrderEmailKey, value);
        }
        public string GetOrderEmail()
        {
            return CurrentRequestData.CurrentContext.Session != null
                           ? CurrentRequestData.CurrentContext.Session[CurrentOrderEmailKey] as string
                           : String.Empty;
        }
        public void SetDiscountCode(string value)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session[CurrentDiscountCodeKey] != null)
                CurrentRequestData.CurrentContext.Session[CurrentDiscountCodeKey] = value;
            else
                CurrentRequestData.CurrentContext.Session.Add(CurrentDiscountCodeKey, value);
        }
        public string GetDiscountCode()
        {
            return CurrentRequestData.CurrentContext.Session != null
                           ? CurrentRequestData.CurrentContext.Session[CurrentDiscountCodeKey] as string
                           : String.Empty;
        }
        public void SetPaymentMethod(string value)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session[CurrentPaymentMethodKey] != null)
                CurrentRequestData.CurrentContext.Session[CurrentPaymentMethodKey] = value;
            else
                CurrentRequestData.CurrentContext.Session.Add(CurrentPaymentMethodKey, value);
        }
        public string GetPaymentMethod()
        {
            return CurrentRequestData.CurrentContext.Session != null
                           ? CurrentRequestData.CurrentContext.Session[CurrentPaymentMethodKey] as string
                           : String.Empty;
        }

        public void SetCountry(int id)
        {
            if (CurrentRequestData.CurrentContext.Session != null && CurrentRequestData.CurrentContext.Session[CurrentCountryIdKey] != null)
                CurrentRequestData.CurrentContext.Session[CurrentCountryIdKey] = id;
            else
                CurrentRequestData.CurrentContext.Session.Add(CurrentCountryIdKey, id);
        }
        public Country GetCountry()
        {
            var id = CurrentRequestData.CurrentContext.Session != null
                                        ? CurrentRequestData.CurrentContext.Session[CurrentCountryIdKey] as int?
                                        : null;

            return id.HasValue
                       ? _session.Get<Country>(id.Value)
                       : null;
        }
    }
}