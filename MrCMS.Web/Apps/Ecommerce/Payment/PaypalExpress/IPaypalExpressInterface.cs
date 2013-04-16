using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PaypalExpress
{
    public interface IPaypalExpressCheckoutService
    {
        string GetRedirectUrl(CartModel cart);
    }
}