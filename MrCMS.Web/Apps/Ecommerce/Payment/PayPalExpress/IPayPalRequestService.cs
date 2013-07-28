using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalRequestService
    {
        SetExpressCheckoutReq GetSetExpressCheckoutRequest(CartModel cart);
        GetExpressCheckoutDetailsReq GetGetExpressCheckoutRequest(string token);
        DoExpressCheckoutPaymentReq GetDoExpressCheckoutRequest(CartModel cart);
    }
}