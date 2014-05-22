using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Models.Payment;

namespace MrCMS.Web.Apps.Ecommerce.Services.CashOnDelivery
{
    public interface ICashOnDeliveryUIService
    {
        CashOnDeliveryPlaceOrderResult TryPlaceOrder();
    }
}