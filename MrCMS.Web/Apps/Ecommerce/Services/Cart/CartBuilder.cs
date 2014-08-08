using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Payment.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
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
        private readonly IGetUserGuid _getUserGuid;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly ICartGuidResetter _cartGuidResetter;

        public CartBuilder(ISession session, IGetUserGuid getUserGuid, IPaymentMethodService paymentMethodService,
         ICartSessionManager cartSessionManager, ICartGuidResetter cartGuidResetter)
        {
            _session = session;
            _getUserGuid = getUserGuid;
            _paymentMethodService = paymentMethodService;
            _cartSessionManager = cartSessionManager;
            _cartGuidResetter = cartGuidResetter;
        }

        public CartModel BuildCart()
        {
            return BuildCart(_getUserGuid.UserGuid);
        }

        public CartModel BuildCart(Guid userGuid)
        {
            List<CartItem> cartItems = GetItems(userGuid);
            DeleteNullProducts(cartItems);
            CartModel cart = new CartModel
            {
                CartGuid = GetCartGuid(userGuid),
                User = CurrentRequestData.CurrentUser,
                UserGuid = userGuid,
                Items = cartItems,
                ShippingAddress = GetShippingAddress(userGuid),
                OrderEmail = GetOrderEmail(userGuid),
                DiscountCode = GetDiscountCode(userGuid),
                Discount = GetDiscount(userGuid),
                AnyStandardPaymentMethodsAvailable = _paymentMethodService.AnyStandardMethodsEnabled(),
                PayPalExpressAvailable = _paymentMethodService.PayPalExpressCheckoutIsEnabled(),
                PayPalExpressPayerId = GetPayPalExpressPayerId(userGuid),
                PayPalExpressToken = GetPayPalExpressToken(userGuid)
            };
            cart.BillingAddressSameAsShippingAddress = cart.RequiresShipping &&
                                                           GetBillingAddressSameAsShippingAddress(userGuid);
            cart.BillingAddress = GetBillingAddress(userGuid, cart.RequiresShipping);
            var availablePaymentMethods = _paymentMethodService.GetAllAvailableMethods(cart);
            cart.AvailablePaymentMethods = availablePaymentMethods;
            cart.PaymentMethod = GetPaymentMethodInfo(userGuid, availablePaymentMethods, cart);

            cartItems.ForEach(item => item.SetDiscountInfo(cart.Discount, cart.DiscountCode));
            if (cart.RequiresShipping)
            {
                cart.ShippingMethod = GetShippingMethod(cart, userGuid);
            }
            throw new NotImplementedException();
            //cart.AvailableShippingMethods = _orderShippingService.AvailableShippingMethods(cart);
            return cart;
        }

        private IPaymentMethod GetPaymentMethodInfo(Guid userGuid, List<IPaymentMethod> availablePaymentMethods, CartModel cart)
        {
            var paymentMethodName = GetPaymentMethod(userGuid) ??
                                    (availablePaymentMethods.Count() == 1
                                        ? availablePaymentMethods.First().SystemName
                                        : null);
            var paymentMethodInfo = _paymentMethodService.GetMethodForCart(paymentMethodName, cart);

            return paymentMethodInfo;
        }

        private Guid GetCartGuid(Guid userGuid)
        {
            var value = _cartSessionManager.GetSessionValue(CartManager.CurrentCartGuid, userGuid, Guid.Empty);
            if (value == Guid.Empty)
            {
                value = _cartGuidResetter.ResetCartGuid(userGuid);
            }
            return value;
        }

        private List<CartItem> GetItems(Guid userGuid)
        {
            return
                _session.QueryOver<CartItem>()
                        .Where(item => item.UserGuid == userGuid)
                        .Cacheable()
                        .List().ToList();
        }

        private void DeleteNullProducts(IEnumerable<CartItem> items)
        {
            foreach (var cartItem in items.Where(x => x.Item == null || x.Item.Product == null))
            {
                CartItem item = cartItem;
                _session.Transact(session => _session.Delete(item));
            }
        }

        private Discount GetDiscount(Guid userGuid)
        {
            return !String.IsNullOrWhiteSpace(GetDiscountCode(userGuid))
                       ? _session.QueryOver<Discount>()
                                 .Where(item => item.Code.IsInsensitiveLike(GetDiscountCode(userGuid), MatchMode.Exact))
                                 .Cacheable()
                                 .SingleOrDefault()
                       : null;
        }

        private Address GetShippingAddress(Guid userGuid)
        {
            var shippingAddress = _cartSessionManager.GetSessionValue<Address>(CartManager.CurrentShippingAddressKey, userGuid);
            if (shippingAddress != null)
            {
                shippingAddress.User = CurrentRequestData.CurrentUser;
            }
            return shippingAddress;
        }

        private bool GetBillingAddressSameAsShippingAddress(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue(CartManager.CurrentBillingAddressSameAsShippingAddressKey, userGuid, true);
        }

        private Address GetBillingAddress(Guid userGuid, bool requiresShipping)
        {
            var billingAddress = GetBillingAddressSameAsShippingAddress(userGuid) && requiresShipping ? GetShippingAddress(userGuid) : _cartSessionManager.GetSessionValue<Address>(CartManager.CurrentBillingAddressKey, userGuid);
            if (billingAddress != null)
            {
                billingAddress.User = CurrentRequestData.CurrentUser;
            }
            return billingAddress;
        }

        private IShippingMethod GetShippingMethod(CartModel cart, Guid userGuid)
        {
            throw new NotImplementedException();
            //var type = _cartSessionManager.GetSessionValue<string>(CartManager.CurrentShippingMethodIdKey, userGuid);
            //if (id > 0)
            //{
            //    var shippingMethod = _session.Get<ShippingMethod>(id);
            //    if (shippingMethod != null)
            //        return shippingMethod;
            //}
            //return _orderShippingService.GetDefaultShippingMethod(cart);
        }

        private string GetOrderEmail(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentOrderEmailKey, userGuid);
        }

        private string GetDiscountCode(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentDiscountCodeKey, userGuid);
        }

        private string GetPaymentMethod(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentPaymentMethodKey, userGuid);
        }

        private string GetPayPalExpressToken(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentPayPalExpressToken, userGuid);
        }

        private string GetPayPalExpressPayerId(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentPayPalExpressPayerId, userGuid);
        }
    }
}