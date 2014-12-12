using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Services
{
    public interface IPaymentMethodService
    {
        bool AnyStandardMethodsEnabled();
        bool PayPalExpressCheckoutIsEnabled();
        List<BasePaymentMethod> GetAllAvailableMethods(CartModel cart);
        BasePaymentMethod GetMethodForCart(string systemName, CartModel cart);
    }
}