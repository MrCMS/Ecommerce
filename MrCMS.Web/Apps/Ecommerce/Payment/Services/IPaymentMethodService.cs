using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Services
{
    public interface IPaymentMethodService
    {
        bool AnyStandardMethodsEnabled();
        bool PayPalExpressCheckoutIsEnabled();
        List<IPaymentMethod> GetAllAvailableMethods(CartModel cart);
        IPaymentMethod GetMethodForCart(string systemName, CartModel cart);
    }
}