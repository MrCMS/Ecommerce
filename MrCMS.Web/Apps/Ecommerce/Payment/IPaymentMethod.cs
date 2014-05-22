using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
        string ControllerName { get; }
        string ActionName { get; }
        PaymentType PaymentType { get; }
        bool Enabled { get; }
        bool UseStandardFlow { get; }
        bool CanUse(CartModel cart);
    }

    public static class PaymentMethodExtensions
    {
        public static string GetUrl(this IPaymentMethod method, UrlHelper urlHelper)
        {
            return urlHelper.Action(method.ActionName, method.ControllerName);
        }
    }

    public abstract class BasePaymentMethod : IPaymentMethod
    {
        public abstract string Name { get; }
        public abstract string SystemName { get; }
        public abstract string ControllerName { get; }
        public abstract string ActionName { get; }
        public abstract PaymentType PaymentType { get; }
        public abstract bool Enabled { get; }

        public bool UseStandardFlow
        {
            get { return PaymentType == PaymentType.Redirection || PaymentType == PaymentType.ServiceBased; }
        }

        public abstract bool CanUse(CartModel cart);
    }

    public interface IPaymentMethodService
    {
        bool AnyStandardMethodsEnabled();
        bool PayPalExpressCheckoutIsEnabled();
        List<IPaymentMethod> GetAllAvailableMethods(CartModel cart);
        IPaymentMethod GetMethodForCart(string systemName, CartModel cart);
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

        public List<IPaymentMethod> GetAllAvailableMethods(CartModel cart)
        {
            return PaymentMethods.FindAll(method => method.Enabled && method.CanUse(cart));
        }

        public IPaymentMethod GetMethodForCart(string systemName, CartModel cart)
        {
            return GetAllAvailableMethods(cart).Find(method => method.SystemName == systemName);
        }
    }

    public enum PaymentType
    {
        PayPalExpress,
        Redirection,
        ServiceBased
    }
}