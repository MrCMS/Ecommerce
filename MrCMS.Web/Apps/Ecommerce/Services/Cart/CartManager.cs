using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartManager : ICartManager, ICartSessionKeyList
    {
        public const string CurrentCartGuid = "current.cart-guid";
        public const string CurrentShippingAddressKey = "current.shipping-address";

        public const string CurrentBillingAddressSameAsShippingAddressKey =
            "current.billing-address-same-as-shipping-address";

        public const string CurrentBillingAddressKey = "current.billing-address";
        public const string CurrentShippingMethodTypeKey = "current.shipping-method-type";
        public const string CurrentOrderEmailKey = "current.order-email";
        public const string CurrentGiftMessageKey = "current.gift-message";
        public const string CurrentPaymentMethodKey = "current.payment-method";
        public const string CurrentPayPalExpressToken = "current.paypal-express-token";
        public const string CurrentPayPalExpressPayerId = "current.paypal-express-payer-id";
        public const string CurrentAppliedGiftCards = "current.applied-gift-cards";

        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;
        private readonly ICartBuilder _cartBuilder;

        public CartManager(ICartBuilder cartBuilder, ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid)
        {
            _cartBuilder = cartBuilder;
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                yield return CurrentCartGuid;
                yield return CurrentShippingAddressKey;
                yield return CurrentBillingAddressSameAsShippingAddressKey;
                yield return CurrentBillingAddressKey;
                yield return CurrentShippingMethodTypeKey;
                yield return CurrentOrderEmailKey;
                yield return CurrentGiftMessageKey;
                yield return CurrentPaymentMethodKey;
                yield return CurrentPayPalExpressToken;
                yield return CurrentPayPalExpressPayerId;
                yield return CurrentAppliedGiftCards;
            }
        }

        public void SetShippingAddress(Address address, Guid? userGuid = null)
        {
            _cartSessionManager.SetSessionValue(CurrentShippingAddressKey, userGuid ?? _getUserGuid.UserGuid, address);
        }

        public void SetShippingMethod(IShippingMethod shippingMethod)
        {
            if (shippingMethod != null)
            {
                _cartSessionManager.SetSessionValue(CurrentShippingMethodTypeKey, _getUserGuid.UserGuid,
                    shippingMethod.TypeName);
            }
            else
            {
                _cartSessionManager.RemoveValue(CurrentShippingMethodTypeKey, _getUserGuid.UserGuid);
            }
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


        public void AddGiftCard(string code)
        {
            var userGuid = _getUserGuid.UserGuid;
            var codes = _cartSessionManager.GetSessionValue(CurrentAppliedGiftCards,userGuid,new List<string>());
            if (!codes.Contains(code))
                codes.Add(code);
            _cartSessionManager.SetSessionValue(CurrentAppliedGiftCards, userGuid, codes);
        }

        public void RemoveGiftCard(string code)
        {
            var userGuid = _getUserGuid.UserGuid;
            var codes = _cartSessionManager.GetSessionValue(CurrentAppliedGiftCards,userGuid,new List<string>());
            if (codes.Contains(code))
                codes.RemoveAll(s => s == code);
            _cartSessionManager.SetSessionValue(CurrentAppliedGiftCards, userGuid, codes);
        }

        public void SetOrderEmail(string email)
        {
            _cartSessionManager.SetSessionValue(CurrentOrderEmailKey, _getUserGuid.UserGuid, email);
        }

        public BasePaymentMethod SetPaymentMethod(string methodName)
        {
            _cartSessionManager.SetSessionValue(CurrentPaymentMethodKey, _getUserGuid.UserGuid, methodName);
            return _cartBuilder.BuildCart().PaymentMethod;
        }

        public void SetPayPalExpressPayerId(string payerId)
        {
            _cartSessionManager.SetSessionValue(CurrentPayPalExpressPayerId, _getUserGuid.UserGuid, payerId);
        }

        public void SetPayPalExpressToken(string token)
        {
            _cartSessionManager.SetSessionValue(CurrentPayPalExpressToken, _getUserGuid.UserGuid, token);
        }

    }
}