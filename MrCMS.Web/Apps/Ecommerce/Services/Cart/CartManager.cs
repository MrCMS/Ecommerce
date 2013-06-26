using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartManager : ICartManager
    {
        private readonly IGetCart _getCart;
        private readonly ISession _session;

        public CartManager(IGetCart getCart, ISession session)
        {
            _getCart = getCart;
            _session = session;
        }

        public void AddToCart(IBuyableItem item, int quantity)
        {
            var cartModel = _getCart.GetCart();

            var existingItem = cartModel.Items.FirstOrDefault(cartItem => cartItem.Item.SKU == item.SKU);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
            {
                existingItem = new CartItem {Item = item, Quantity = quantity, UserGuid=CurrentRequestData.UserGuid};
                cartModel.Items.Add(existingItem);
            }
            _session.Transact(session => session.SaveOrUpdate(existingItem));
        }

        public void Delete(CartItem item)
        {
            var cartModel = _getCart.GetCart();

            cartModel.Items.Remove(item);
            _session.Transact(session => session.Delete(item));
        }

        public void UpdateQuantity(CartItem item, int quantity)
        {
            item.Quantity = quantity;

            _session.Transact(session => session.Update(item));
        }

        public void EmptyBasket()
        {
            var cartModel = _getCart.GetCart();

            cartModel.Items.ForEach(Delete);
        }
    }
}