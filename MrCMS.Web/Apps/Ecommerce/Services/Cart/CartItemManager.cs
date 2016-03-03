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
        private readonly IGetExistingCartItem _getExistingCartItem;

        public CartItemManager(ICartBuilder cartBuilder, ISession session, IGetUserGuid getUserGuid, IGetExistingCartItem getExistingCartItem)
        {
            _cartBuilder = cartBuilder;
            _session = session;
            _getUserGuid = getUserGuid;
            _getExistingCartItem = getExistingCartItem;
        }

        public void AddToCart(AddToCartModel model)
        {
            int quantity = model.Quantity;
            ProductVariant productVariant = model.ProductVariant;
            var data = model.Data;

            var cart = _cartBuilder.BuildCart();
            CartItem cartItem = _getExistingCartItem.GetExistingItem(cart, productVariant, data);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                _session.Transact(session => session.Update(cartItem));
            }
            else
            {
                cartItem = new CartItem
                {
                    Item = productVariant,
                    Quantity = quantity,
                    UserGuid = _getUserGuid.UserGuid,
                    Data = data
                };
                _session.Transact(session => session.Save(cartItem));
                var cartItemData = cartItem.GetCartItemData();
                cart.Items.Add(cartItemData);
            }
        }

        public void Delete(CartItem item)
        {
            var cart = _cartBuilder.BuildCart();
            _session.Transact(session => session.Delete(item));
            cart.Items.RemoveAll(data => data.Id == item.Id);
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
                    var cartItemData = cart.Items.FirstOrDefault(item => item.Id == value.ItemId);

                    if (cartItemData == null) continue;

                    var cartItem = _session.Get<CartItem>(cartItemData.Id);

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