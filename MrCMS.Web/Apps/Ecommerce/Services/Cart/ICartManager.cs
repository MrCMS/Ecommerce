using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartManager
    {
        void AddToCart(ProductVariant item, int quantity);
        void Delete(CartItem item);
        void UpdateQuantity(CartItem item, int quantity);
        void UpdateQuantities(List<CartUpdateValue> quantities);
        void EmptyBasket();

        void SetOrderEmail(string email);
        void SetShippingAddress(Address address);
        void SetBillingAddress(Address address);
        void SetBillingAddressSameAsShippingAddress(bool value);
        void SetDiscountCode(string code);
        void SetPaymentMethod(string methodName);
        void SetShippingInfo(ShippingCalculation shippingCalculation);
        void SetPayPalExpressInfo(string token, string payerId);
    }
}