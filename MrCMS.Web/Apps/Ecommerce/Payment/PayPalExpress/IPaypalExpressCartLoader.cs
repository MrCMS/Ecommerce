using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPaypalExpressCartLoader
    {
        CartModel GetCart(string token);
    }
}