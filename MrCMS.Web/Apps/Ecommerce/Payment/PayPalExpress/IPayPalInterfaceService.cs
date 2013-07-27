using Lucene.Net.Util;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using PayPal.Authentication;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalInterfaceService
    {
        SetExpressCheckoutResponseType SetExpressCheckout(CartModel cart);
        GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(string token);
        DoExpressCheckoutPaymentResponseType DoExpressCheckout(CartModel cart);
    }
}