using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Website;
using NHibernate;

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

        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;
        private readonly ICartBuilder _cartBuilder;
        private readonly ISession _session;
        private readonly IEnumerable<ICartSessionKeyList> _sessionKeyLists;

        public CartManager(ICartBuilder cartBuilder, ISession session, ICartSessionManager cartSessionManager,
            IEnumerable<ICartSessionKeyList> sessionKeyLists, IGetUserGuid getUserGuid)
        {
            _cartBuilder = cartBuilder;
            _session = session;
            _cartSessionManager = cartSessionManager;
            _sessionKeyLists = sessionKeyLists;
            _getUserGuid = getUserGuid;
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
                foreach (string key in _sessionKeyLists.SelectMany(keyList => keyList.Keys))
                {
                    yield return key;
                }
            }
        }

        public void AddToCart(AddToCartModel model)
        {
            AddToCart(model.ProductVariant, model.Quantity);
        }

        public void Delete(CartItem item)
        {
            var cart = _cartBuilder.BuildCart();
            _session.Transact(session => session.Delete(item));
            cart.Items.Remove(item);
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
                                  var cart = _cartBuilder.BuildCart();
                                  foreach (CartUpdateValue value in quantities)
                                  {
                                      CartItem cartItem = cart.Items.FirstOrDefault(item => item.Id == value.ItemId);

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
            var cart = _cartBuilder.BuildCart();
            foreach (CartItem item in cart.Items)
            {
                CartItem item1 = item;
                _session.Transact(session => session.Delete(item1));
            }
            cart.Items.Clear();
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
            _cartSessionManager.SetSessionValue(CurrentBillingAddressSameAsShippingAddressKey, _getUserGuid.UserGuid,
                value);
        }

        public void SetDiscountCode(string code)
        {
            _cartSessionManager.SetSessionValue(CurrentDiscountCodeKey, _getUserGuid.UserGuid, code);
        }

        public void SetOrderEmail(string email)
        {
            _cartSessionManager.SetSessionValue(CurrentOrderEmailKey, _getUserGuid.UserGuid, email);
        }

        public IPaymentMethod SetPaymentMethod(string methodName)
        {
            _cartSessionManager.SetSessionValue(CurrentPaymentMethodKey, _getUserGuid.UserGuid, methodName);
            return _cartBuilder.BuildCart().PaymentMethod;
        }

        public void SetShippingInfo(ShippingCalculation shippingCalculation)
        {
            if (shippingCalculation == null) return;

            if (shippingCalculation.ShippingMethod != null)
                _cartSessionManager.SetSessionValue(CurrentShippingMethodIdKey, _getUserGuid.UserGuid,
                    shippingCalculation.ShippingMethod.Id);
            if (shippingCalculation.Country != null)
                SetCountry(shippingCalculation.Country);
        }

        public void SetCountry(Country country)
        {
            if (country != null)
                _cartSessionManager.SetSessionValue(CurrentCountryIdKey, _getUserGuid.UserGuid, country.Id);
        }

        public void SetPayPalExpressInfo(string token, string payerId)
        {
            _cartSessionManager.SetSessionValue(CurrentPayPalExpressToken, _getUserGuid.UserGuid, token);
            _cartSessionManager.SetSessionValue(CurrentPayPalExpressPayerId, _getUserGuid.UserGuid, payerId);
        }

        private void AddToCart(ProductVariant item, int quantity)
        {
            var cart = _cartBuilder.BuildCart();
            CartItem existingItem = cart.Items.FirstOrDefault(cartItem => cartItem.Item.SKU == item.SKU);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
            {
                existingItem = new CartItem { Item = item, Quantity = quantity, UserGuid = CurrentRequestData.UserGuid };
                cart.Items.Add(existingItem);
            }
            _session.Transact(session => session.SaveOrUpdate(existingItem));
        }
    }
}