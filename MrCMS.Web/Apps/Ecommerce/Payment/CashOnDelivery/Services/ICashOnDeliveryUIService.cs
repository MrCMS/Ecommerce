using MrCMS.Web.Apps.Ecommerce.Models.Payment;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CashOnDelivery.Services
{
    public interface ICashOnDeliveryUIService
    {
        CashOnDeliveryPlaceOrderResult TryPlaceOrder();
    }
}