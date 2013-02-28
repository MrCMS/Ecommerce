using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface ICartManager
    {
        void AddToCart(ICanAddToCart item, int quantity);
        void Delete(CartItem item);
        void UpdateQuantity(CartItem item, int quantity);
        void EmptyBasket();
    }
}