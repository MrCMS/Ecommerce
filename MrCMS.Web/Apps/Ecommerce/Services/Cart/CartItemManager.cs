using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartItemManager : ICartItemManager
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly ISession _session;
        private readonly IGetUserGuid _getUserGuid;

        public CartItemManager(ICartBuilder cartBuilder, ISession session, IGetUserGuid getUserGuid)
        {
            _cartBuilder = cartBuilder;
            _session = session;
            _getUserGuid = getUserGuid;
        }

        public void AddToCart(AddToCartModel model)
        {
            int quantity = model.Quantity;
            ProductVariant productVariant = model.ProductVariant;
            var data = model.Data;

            var cart = _cartBuilder.BuildCart();
            CartItem cartItem = GetExistingItem(cart, productVariant, data);
            if (cartItem != null)
                cartItem.Quantity += quantity;
            else
            {
                cartItem = new CartItem
                {
                    Item = productVariant,
                    Quantity = quantity,
                    UserGuid = _getUserGuid.UserGuid,
                    Data = data
                };
                cart.Items.Add(cartItem);
            }
            _session.Transact(session => session.SaveOrUpdate(cartItem));
        }

        private static CartItem GetExistingItem(CartModel cart, ProductVariant item, string data)
        {
            return
                cart.Items.FirstOrDefault(
                    cartItem => cartItem.Item.SKU == item.SKU && cartItem.Data == data);
        }

        public void Delete(CartItem item)
        {
            var cart = _cartBuilder.BuildCart();
            _session.Transact(session => session.Delete(item));
            cart.Items.Remove(item);
        }

        public void UpdateQuantity(CartItem item, int quantity)
        {
            item.Quantity = quantity;

            _session.Transact(session => session.Update(item));
        }

        public void UpdateQuantities(List<CartUpdateValue> quantities)
        {
            _session.Transact(session =>
            {
                var cart = _cartBuilder.BuildCart();
                foreach (CartUpdateValue value in quantities)
                {
                    CartItem cartItem = cart.Items.FirstOrDefault(item => item.Id == value.ItemId);

                    if (cartItem == null) continue;

                    if (value.Quantity <= 0)
                        session.Delete(cartItem);
                    else
                    {
                        cartItem.Quantity = value.Quantity;
                        session.Update(cartItem);
                    }
                }
            });
        }
    }
}