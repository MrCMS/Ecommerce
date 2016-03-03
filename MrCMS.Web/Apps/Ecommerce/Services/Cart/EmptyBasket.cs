using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class EmptyBasket : IEmptyBasket
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly IClearCartSessionKeys _clearCartSessionKeys;
        private readonly ISession _session;

        public EmptyBasket(ICartBuilder cartBuilder, ISession session, IClearCartSessionKeys clearCartSessionKeys)
        {
            _cartBuilder = cartBuilder;
            _session = session;
            _clearCartSessionKeys = clearCartSessionKeys;
        }

        public void Empty()
        {
            var cart = _cartBuilder.BuildCart();
            foreach (var item in cart.Items)
            {
                var item1 = _session.Get<CartItem>(item.Id);
                if (item1 != null)
                    _session.Transact(session => session.Delete(item1));
            }
            cart.Items.Clear();
            _clearCartSessionKeys.Clear();
        }
    }
}