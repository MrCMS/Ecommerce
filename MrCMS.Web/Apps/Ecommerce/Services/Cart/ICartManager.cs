using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartManager
    {
        void AddToCart(ProductVariant item, int quantity);
        void Delete(CartItem item);
        void UpdateQuantity(CartItem item, int quantity);
        void EmptyBasket();

        void SetOrderEmail(string email);
        void SetShippingAddress(Address address);
        void SetBillingAddress(Address address);
        void SetDiscountCode(string code);
        void SetPaymentMethod(string methodName);
        void SetShippingInfo(ShippingCalculation shippingCalculation);
    }
}