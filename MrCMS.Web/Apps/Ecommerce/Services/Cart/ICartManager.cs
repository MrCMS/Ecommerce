using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartManager
    {
        void SetOrderEmail(string email);
        void SetShippingAddress(Address address);
        void SetShippingMethod(IShippingMethod shippingMethod);
        void SetBillingAddress(Address address);
        void SetBillingAddressSameAsShippingAddress(bool value);
        void AddGiftCard(string code);
        void RemoveGiftCard(string code);
        IPaymentMethod SetPaymentMethod(string methodName);
        void SetPayPalExpressInfo(string token, string payerId);
    }
}