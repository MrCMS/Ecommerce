using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;

namespace MrCMS.Web.Apps.Ecommerce.Payment
{
    public interface IPaymentMethod
    {
        string Name { get; }
        string SystemName { get; }
        PaymentType PaymentType { get; }
        bool Enabled { get; }
        bool UseStandardFlow { get; }
    }

    public abstract class BasePaymentMethod : IPaymentMethod
    {
        public abstract string Name { get; }
        public abstract string SystemName { get; }
        public abstract PaymentType PaymentType { get; }
        public abstract bool Enabled { get; }

        public bool UseStandardFlow
        {
            get { return PaymentType == PaymentType.Redirection || PaymentType == PaymentType.ServiceBased; }
        }
    }

    public interface IPaymentMethodService
    {
        bool AnyStandardMethodsEnabled();
        bool PayPalExpressCheckoutIsEnabled();
        List<IPaymentMethod> GetAllAvailableMethods();
    }

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
            return PaymentMethods.Any(method => method.Enabled && method.UseStandardFlow);
        }

        public bool PayPalExpressCheckoutIsEnabled()
        {
            return PaymentMethods.OfType<PayPalExpressCheckoutPaymentMethod>().First().Enabled;
        }

        public List<IPaymentMethod> GetAllAvailableMethods()
        {
            return PaymentMethods.FindAll(method => method.Enabled);
        }
    }

    public enum PaymentType
    {
        PayPalExpress,
        Redirection,
        ServiceBased
    }
}