using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Helpers;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IKernel _kernel;

        public PaymentMethodService(IKernel kernel)
        {
            _kernel = kernel;
        }

        private List<BasePaymentMethod> _paymentMethods;

        public List<BasePaymentMethod> PaymentMethods
        {
            get
            {
                return
                    _paymentMethods =
                        _paymentMethods ??
                        TypeHelper.GetAllConcreteTypesAssignableFrom<BasePaymentMethod>()
                            .Select(type => _kernel.Get(type))
                            .Cast<BasePaymentMethod>()
                            .ToList();
            }
        }

        public bool AnyStandardMethodsEnabled()
        {
            return PaymentMethods.Any(method => method.Enabled && method.UseStandardFlow());
        }

        public bool PayPalExpressCheckoutIsEnabled()
        {
            return PaymentMethods.OfType<PayPalExpressCheckoutPaymentMethod>().First().Enabled;
        }

        public List<BasePaymentMethod> GetAllAvailableMethods(CartModel cart)
        {
            return PaymentMethods.FindAll(method => method.Enabled && method.CanUse(cart));
        }

        public BasePaymentMethod GetMethodForCart(string systemName, CartModel cart)
        {
            return GetAllAvailableMethods(cart).Find(method => method.SystemName == systemName);
        }
    }
}