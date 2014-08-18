using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class EmptyBasket : IOnOrderPlaced
    {
        private readonly IEmptyBasket _emptyBasket;

        public EmptyBasket(IEmptyBasket emptyBasket)
        {
            _emptyBasket = emptyBasket;
        }

        public void Execute(OrderPlacedArgs args)
        {
            _emptyBasket.Empty();
        }
    }
}