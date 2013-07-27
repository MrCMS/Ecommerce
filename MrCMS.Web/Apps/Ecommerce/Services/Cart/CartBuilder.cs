using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website;
using NHibernate;
using System;
using NHibernate.Criterion;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartBuilder : ICartBuilder
    {
        private readonly ISession _session;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IOrderShippingService _orderShippingService;

        public CartBuilder(ISession session, IPaymentMethodService paymentMethodService, IOrderShippingService orderShippingService)
        {
            _session = session;
            _paymentMethodService = paymentMethodService;
            _orderShippingService = orderShippingService;
        }

        public CartModel BuildCart()
        {
            //var address = GetShippingAddress() ?? new Address();
            //if (GetCountry() != null)
            //    address.Country = GetCountry();
            var availablePaymentMethods = _paymentMethodService.GetAllAvailableMethods();

            //remove deleted product items from cart
            var cartItems = GetItems();
            DeleteNullProducts(cartItems);

            var cart = new CartModel
                           {
                               User = CurrentRequestData.CurrentUser,
                               UserGuid = CurrentRequestData.UserGuid,
                               Items = cartItems,
                               ShippingAddress = GetShippingAddress(),
                               BillingAddressSameAsShippingAddress = GetBillingAddressSameAsShippingAddress(),
                               BillingAddress = GetBillingAddress(),
                               Country = GetCountry(),
                               OrderEmail = GetOrderEmail(),
                               DiscountCode = GetDiscountCode(),
                               Discount = GetDiscount(),
                               AnyStandardPaymentMethodsAvailable = _paymentMethodService.AnyStandardMethodsEnabled(),
                               PayPalExpressAvailable = _paymentMethodService.PayPalExpressCheckoutIsEnabled(),
                               AvailablePaymentMethods = availablePaymentMethods,
                               PaymentMethod = GetPaymentMethod() ?? (availablePaymentMethods.Count() == 1 ? availablePaymentMethods.First().SystemName : null),
                               PayPalExpressPayerId = GetPayPalExpressPayerId(),
                               PayPalExpressToken = GetPayPalExpressToken(),
                           };

            cart.ShippingMethod = GetShippingMethod(cart);
            return cart;
        }

        private List<CartItem> GetItems()
        {
            return
                _session.QueryOver<CartItem>()
                        .Where(item => item.UserGuid == CurrentRequestData.UserGuid)
                        .Cacheable()
                        .List().ToList();
        }

        private void DeleteNullProducts(IEnumerable<CartItem> items)
        {
            foreach (var cartItem in items.Where(x => x.Item == null))
            {
                CartItem item = cartItem;
                _session.Transact(session => _session.Delete(item));
            }
        }

        private Discount GetDiscount()
        {
            return !String.IsNullOrWhiteSpace(GetDiscountCode())
                       ? _session.QueryOver<Discount>()
                                 .Where(item => item.Code.IsInsensitiveLike(GetDiscountCode(), MatchMode.Exact))
                                 .Cacheable()
                                 .SingleOrDefault()
                       : null;
        }

        private Address GetShippingAddress()
        {
            var shippingAddress = GetSessionValue<Address>(CartManager.CurrentShippingAddressKey);
            if (shippingAddress != null)
            {
                shippingAddress.Country = GetCountry();
            }
            return shippingAddress;
        }

        private bool GetBillingAddressSameAsShippingAddress()
        {
            return GetSessionValue<bool>(CartManager.CurrentBillingAddressSameAsShippingAddressKey, true);
        }
        private Address GetBillingAddress()
        {
            var billingAddress = GetBillingAddressSameAsShippingAddress() ? GetShippingAddress() : GetSessionValue<Address>(CartManager.CurrentBillingAddressKey);
            if (billingAddress != null)
            {
                billingAddress.Country = GetCountry();
            }
            return billingAddress;
        }

        private ShippingMethod GetShippingMethod(CartModel cart)
        {
            var id = GetSessionValue<int>(CartManager.CurrentShippingMethodIdKey);

            return _session.Get<ShippingMethod>(id) ??
                   _orderShippingService.GetDefaultShippingMethod(cart);
        }

        private string GetOrderEmail()
        {
            return GetSessionValue<string>(CartManager.CurrentOrderEmailKey);
        }

        private string GetDiscountCode()
        {
            return GetSessionValue<string>(CartManager.CurrentDiscountCodeKey);
        }

        private string GetPaymentMethod()
        {
            return GetSessionValue<string>(CartManager.CurrentPaymentMethodKey);
        }

        private string GetPayPalExpressToken()
        {
            return GetSessionValue<string>(CartManager.CurrentPayPalExpressToken);
        }

        private string GetPayPalExpressPayerId()
        {
            return GetSessionValue<string>(CartManager.CurrentPayPalExpressPayerId);
        }

        private Country GetCountry()
        {
            var id = GetSessionValue<int>(CartManager.CurrentCountryIdKey);

            return _session.Get<Country>(id) ??
                   _session.QueryOver<Country>().Cacheable().Take(1).SingleOrDefault();
        }

        private T GetSessionValue<T>(string key, T defaultValue = default(T))
        {
            if (CurrentRequestData.CurrentContext.Session != null)
            {
                try { return (T)Convert.ChangeType(CurrentRequestData.CurrentContext.Session[key], typeof(T)); }
                catch { }
            }
            return defaultValue;
        }
    }
}