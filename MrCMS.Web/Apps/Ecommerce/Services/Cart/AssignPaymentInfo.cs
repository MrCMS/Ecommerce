using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Payment.Services;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignPaymentInfo : IAssignPaymentInfo
    {
        private readonly IGetBillingAddressSameAsShippingAddress _billingAddressSameAsShippingAddress;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IGetShippingAddress _getShippingAddress;
        private readonly IPaymentMethodService _paymentMethodService;

        public AssignPaymentInfo(IPaymentMethodService paymentMethodService, IGetCurrentUser getCurrentUser,
            IGetBillingAddressSameAsShippingAddress billingAddressSameAsShippingAddress,
            IGetShippingAddress getShippingAddress, ICartSessionManager cartSessionManager)
        {
            _paymentMethodService = paymentMethodService;
            _getCurrentUser = getCurrentUser;
            _billingAddressSameAsShippingAddress = billingAddressSameAsShippingAddress;
            _getShippingAddress = getShippingAddress;
            _cartSessionManager = cartSessionManager;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            cart.AnyStandardPaymentMethodsAvailable = _paymentMethodService.AnyStandardMethodsEnabled();
            cart.PayPalExpressAvailable = _paymentMethodService.PayPalExpressCheckoutIsEnabled();
            cart.PayPalExpressPayerId = GetPayPalExpressPayerId(userGuid);
            cart.PayPalExpressToken = GetPayPalExpressToken(userGuid);
            cart.BillingAddress = GetBillingAddress(cart, userGuid);
            List<IPaymentMethod> availablePaymentMethods = _paymentMethodService.GetAllAvailableMethods(cart);
            cart.AvailablePaymentMethods = availablePaymentMethods;
            cart.PaymentMethod = GetPaymentMethodInfo(userGuid, availablePaymentMethods, cart);
            return cart;
        }

        private IPaymentMethod GetPaymentMethodInfo(Guid userGuid, List<IPaymentMethod> availablePaymentMethods,
            CartModel cart)
        {
            string paymentMethodName = GetPaymentMethod(userGuid) ??
                                       (availablePaymentMethods.Count() == 1
                                           ? availablePaymentMethods.First().SystemName
                                           : null);
            IPaymentMethod paymentMethodInfo = _paymentMethodService.GetMethodForCart(paymentMethodName, cart);

            return paymentMethodInfo;
        }


        private Address GetBillingAddress(CartModel cart, Guid userGuid)
        {
            Address billingAddress = _billingAddressSameAsShippingAddress.Get(cart, userGuid) && cart.RequiresShipping
                ? _getShippingAddress.Get(userGuid)
                : _cartSessionManager.GetSessionValue<Address>(CartManager.CurrentBillingAddressKey, userGuid);
            if (billingAddress != null)
            {
                billingAddress.User = _getCurrentUser.Get();
            }
            return billingAddress;
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