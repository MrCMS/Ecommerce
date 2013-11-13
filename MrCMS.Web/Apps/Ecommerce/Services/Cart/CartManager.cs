using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartManager : ICartManager
    {
        public const string CurrentCartGuid = "current.cart-guid";
        public const string CurrentShippingAddressKey = "current.shipping-address";
        public const string CurrentBillingAddressSameAsShippingAddressKey =
            "current.billing-address-same-as-shipping-address";
        public const string CurrentBillingAddressKey = "current.billing-address";
        public const string CurrentShippingMethodIdKey = "current.shipping-method-id";
        public const string CurrentOrderEmailKey = "current.order-email";
        public const string CurrentDiscountCodeKey = "current.discount-code";
        public const string CurrentPaymentMethodKey = "current.payment-method";
        public const string CurrentCountryIdKey = "current.country-id";
        public const string CurrentPayPalExpressToken = "current.paypal-express-token";
        public const string CurrentPayPalExpressPayerId = "current.paypal-express-payer-id";

        private readonly CartModel _cart;
        private readonly ISession _session;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IEnumerable<ICartSessionKeyList> _sessionKeyLists;
        private readonly IGetUserGuid _getUserGuid;

        public CartManager(CartModel cart, ISession session, ICartSessionManager cartSessionManager, IEnumerable<ICartSessionKeyList> sessionKeyLists, IGetUserGuid getUserGuid)
        {
            _cart = cart;
            _session = session;
            _cartSessionManager = cartSessionManager;
            _sessionKeyLists = sessionKeyLists;
            _getUserGuid = getUserGuid;
        }

        public void AddToCart(ProductVariant item, int quantity)
        {
            var existingItem = _cart.Items.FirstOrDefault(cartItem => cartItem.Item.SKU == item.SKU);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
            {
                existingItem = new CartItem { Item = item, Quantity = quantity, UserGuid = CurrentRequestData.UserGuid };
                _cart.Items.Add(existingItem);
            }
            _session.Transact(session => session.SaveOrUpdate(existingItem));
        }

        public void Delete(CartItem item)
        {
            _session.Transact(session => session.Delete(item));
            _cart.Items.Remove(item);
        }

        public void UpdateQuantity(CartItem item, int quantity)
        {
            item.Quantity = quantity;

            _session.Transact(session => session.Update(item));
        }

        public void UpdateQuantities(List<CartUpdateValue> quantities)
        {
            _session.Transact(session =>
            {
                foreach (var value in quantities)
                {
                    var cartItem = _cart.Items.FirstOrDefault(item => item.Id == value.ItemId);

                    if (cartItem != null)
                    {
                        if (value.Quantity <= 0)
                            session.Delete(cartItem);
                        else
                        {
                            cartItem.Quantity = value.Quantity;
                            session.Update(cartItem);
                        }
                    }
                }
            });
        }

        public void EmptyBasket()
        {
            foreach (var item in _cart.Items)
            {
                _session.Transact(session => session.Delete(item));
            }
            _cart.Items.Clear();
            CartKeys.ForEach(s => _cartSessionManager.RemoveValue(s, _getUserGuid.UserGuid));
        }

        public void SetShippingAddress(Address address)
        {
            _cartSessionManager.SetSessionValue(CurrentShippingAddressKey, _getUserGuid.UserGuid, address);
        }

        public void SetBillingAddress(Address address)
        {
            _cartSessionManager.SetSessionValue(CurrentBillingAddressKey, _getUserGuid.UserGuid, address);
        }

        public void SetBillingAddressSameAsShippingAddress(bool value)
        {
            _cartSessionManager.SetSessionValue(CurrentBillingAddressSameAsShippingAddressKey, _getUserGuid.UserGuid, value);
        }

        public void SetDiscountCode(string code)
        {
            _cartSessionManager.SetSessionValue(CurrentDiscountCodeKey, _getUserGuid.UserGuid, code);
        }

        public void SetOrderEmail(string email)
        {
            _cartSessionManager.SetSessionValue(CurrentOrderEmailKey, _getUserGuid.UserGuid, email);
        }

        public void SetPaymentMethod(string methodName)
        {
            _cartSessionManager.SetSessionValue(CurrentPaymentMethodKey, _getUserGuid.UserGuid, methodName);
        }

        public void SetShippingInfo(ShippingCalculation shippingCalculation)
        {
            if (shippingCalculation == null) return;

            if (shippingCalculation.ShippingMethod != null)
                _cartSessionManager.SetSessionValue(CurrentShippingMethodIdKey, _getUserGuid.UserGuid, shippingCalculation.ShippingMethod.Id);
            if (shippingCalculation.Country != null)
                SetCountry(shippingCalculation.Country);
        }

        public void SetCountry(Country country)
        {
            if (country != null) _cartSessionManager.SetSessionValue(CurrentCountryIdKey, _getUserGuid.UserGuid, country.Id);
        }

        public void SetPayPalExpressInfo(string token, string payerId)
        {
            _cartSessionManager.SetSessionValue(CurrentPayPalExpressToken, _getUserGuid.UserGuid, token);
            _cartSessionManager.SetSessionValue(CurrentPayPalExpressPayerId, _getUserGuid.UserGuid, payerId);
        }
        public IEnumerable<string> CartKeys
        {
            get
            {
                yield return CurrentCartGuid;
                yield return CurrentShippingAddressKey;
                yield return CurrentBillingAddressSameAsShippingAddressKey;
                yield return CurrentBillingAddressKey;
                yield return CurrentShippingMethodIdKey;
                yield return CurrentOrderEmailKey;
                yield return CurrentDiscountCodeKey;
                yield return CurrentPaymentMethodKey;
                yield return CurrentCountryIdKey;
                yield return CurrentPayPalExpressToken;
                yield return CurrentPayPalExpressPayerId;
                foreach (var key in _sessionKeyLists.SelectMany(keyList => keyList.Keys))
                {
                    yield return key;
                }
            }
        }
    }
}