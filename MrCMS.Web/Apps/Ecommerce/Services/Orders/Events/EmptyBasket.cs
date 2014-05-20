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

        public void Execute(OrderPlacedArgs args)
        {
            _cartManager.EmptyBasket();
        }
    }
}