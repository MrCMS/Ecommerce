using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class EmptyBasket : IEmptyBasket
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly ISession _session;
        private readonly IClearCartSessionKeys _clearCartSessionKeys;

        public EmptyBasket(ICartBuilder cartBuilder, ISession session, IClearCartSessionKeys clearCartSessionKeys)
        {
            _cartBuilder = cartBuilder;
            _session = session;
            _clearCartSessionKeys = clearCartSessionKeys;
        }

        public void Empty()
        {
            var cart = _cartBuilder.BuildCart();
            foreach (CartItem item in cart.Items)
            {
                CartItem item1 = item;
                _session.Transact(session => session.Delete(item1));
            }
            cart.Items.Clear();
            _clearCartSessionKeys.Clear();
        }
    }
}