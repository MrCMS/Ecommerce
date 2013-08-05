using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class EmptyBasket : IOnOrderPlaced
    {
        private readonly ICartManager _cartManager;

        public EmptyBasket(ICartManager cartManager)
        {
            _cartManager = cartManager;
        }

        public int Order { get { return 10; } }
        public void OnOrderPlaced(Order order)
        {
            _cartManager.EmptyBasket();
        }
    }
}