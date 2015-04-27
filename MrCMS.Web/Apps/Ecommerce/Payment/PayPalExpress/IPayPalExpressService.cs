using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalExpressService
    {
        SetExpressCheckoutResponse GetSetExpressCheckoutRedirectUrl(CartModel cart);
        GetExpressCheckoutResponse ProcessReturn(string token);
        DoExpressCheckoutPaymentResponse DoExpressCheckout(CartModel cart);
        void Reset();
    }
}