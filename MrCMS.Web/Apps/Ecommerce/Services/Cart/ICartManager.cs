using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartManager
    {
        void AddToCart(AddToCartModel model);
        void Delete(CartItem item);
        void UpdateQuantity(CartItem item, int quantity);
        void UpdateQuantities(List<CartUpdateValue> quantities);
        void EmptyBasket();

        void SetOrderEmail(string email);
        void SetShippingAddress(Address address);
        void SetBillingAddress(Address address);
        void SetBillingAddressSameAsShippingAddress(bool value);
        void SetDiscountCode(string code);
        IPaymentMethod SetPaymentMethod(string methodName);
        void SetShippingInfo(ShippingCalculation shippingCalculation);
        void SetCountry(Country country);
        void SetPayPalExpressInfo(string token, string payerId);
    }
}