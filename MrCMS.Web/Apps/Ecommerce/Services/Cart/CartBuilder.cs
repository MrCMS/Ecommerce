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
        private readonly ICartSessionManager _cartSessionManager;

        public CartBuilder(ISession session, IPaymentMethodService paymentMethodService, IOrderShippingService orderShippingService, ICartSessionManager cartSessionManager)
        {
            _session = session;
            _paymentMethodService = paymentMethodService;
            _orderShippingService = orderShippingService;
            _cartSessionManager = cartSessionManager;
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
                               CartGuid = GetCartGuid(),
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

        private Guid GetCartGuid()
        {
            var value = _cartSessionManager.GetSessionValue(CartManager.CurrentCartGuid, Guid.Empty);
            if (value == Guid.Empty)
            {
                value = Guid.NewGuid();
                _cartSessionManager.SetSessionValue(CartManager.CurrentCartGuid, value);
            }
            return value;
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
            var shippingAddress = _cartSessionManager.GetSessionValue<Address>(CartManager.CurrentShippingAddressKey);
            if (shippingAddress != null)
            {
                shippingAddress.Country = GetCountry();
            }
            return shippingAddress;
        }

        private bool GetBillingAddressSameAsShippingAddress()
        {
            return _cartSessionManager.GetSessionValue(CartManager.CurrentBillingAddressSameAsShippingAddressKey, true);
        }

        private Address GetBillingAddress()
        {
            var billingAddress = GetBillingAddressSameAsShippingAddress() ? GetShippingAddress() : _cartSessionManager.GetSessionValue<Address>(CartManager.CurrentBillingAddressKey);
            if (billingAddress != null)
            {
                billingAddress.Country = GetCountry();
            }
            return billingAddress;
        }

        private ShippingMethod GetShippingMethod(CartModel cart)
        {
            var id = _cartSessionManager.GetSessionValue<int>(CartManager.CurrentShippingMethodIdKey);

            return _session.Get<ShippingMethod>(id) ??
                   _orderShippingService.GetDefaultShippingMethod(cart);
        }

        private string GetOrderEmail()
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentOrderEmailKey);
        }

        private string GetDiscountCode()
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentDiscountCodeKey);
        }

        private string GetPaymentMethod()
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentPaymentMethodKey);
        }

        private string GetPayPalExpressToken()
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentPayPalExpressToken);
        }

        private string GetPayPalExpressPayerId()
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentPayPalExpressPayerId);
        }

        private Country GetCountry()
        {
            var id = _cartSessionManager.GetSessionValue<int>(CartManager.CurrentCountryIdKey);

            return _session.Get<Country>(id) ??
                   _session.QueryOver<Country>().Cacheable().Take(1).SingleOrDefault();
        }
    }
}