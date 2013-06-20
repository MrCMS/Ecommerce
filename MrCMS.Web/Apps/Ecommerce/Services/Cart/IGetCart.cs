using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetCart
    {
        CartModel GetCart();

        void SetOrderEmail(string value);
        string GetOrderEmail();
        Address GetShippingAddress();
        void SetShippingAddress(Address address);
        Address GetBillingAddress();
        void SetBillingAddress(Address address);
        ShippingMethod GetShippingMethod();
        void SetShippingMethod(int shippingMethodId);
    }
}