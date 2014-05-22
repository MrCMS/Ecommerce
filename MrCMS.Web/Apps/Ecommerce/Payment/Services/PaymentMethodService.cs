using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Helpers;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private List<IPaymentMethod> _paymentMethods;

        public List<IPaymentMethod> PaymentMethods
        {
            get
            {
                return
                    _paymentMethods =
                        _paymentMethods ??
                        TypeHelper.GetAllConcreteTypesAssignableFrom<IPaymentMethod>()
                            .Select(Activator.CreateInstance)
                            .Cast<IPaymentMethod>()
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

        public List<IPaymentMethod> GetAllAvailableMethods(CartModel cart)
        {
            return PaymentMethods.FindAll(method => method.Enabled && method.CanUse(cart));
        }

        public IPaymentMethod GetMethodForCart(string systemName, CartModel cart)
        {
            return GetAllAvailableMethods(cart).Find(method => method.SystemName == systemName);
        }
    }
}