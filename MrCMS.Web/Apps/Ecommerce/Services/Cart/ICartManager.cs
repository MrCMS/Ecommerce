using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartManager
    {
        void SetOrderEmail(string email);
        void SetShippingAddress(Address address, Guid? userGuid = null);
        void SetShippingMethod(IShippingMethod shippingMethod);
        void SetBillingAddress(Address address);
        void SetBillingAddressSameAsShippingAddress(bool value);
        void AddGiftCard(string code);
        void RemoveGiftCard(string code);
        BasePaymentMethod SetPaymentMethod(string methodName);
        
        void SetPayPalExpressPayerId(string payerId);
        void SetPayPalExpressToken(string token);
        void ResetPayPalExpress();
    }
}