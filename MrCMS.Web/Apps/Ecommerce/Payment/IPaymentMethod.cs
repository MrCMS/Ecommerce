using MrCMS.Web.Apps.Ecommerce.Models;

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
        bool CanUse(CartModel cart);
    }
}