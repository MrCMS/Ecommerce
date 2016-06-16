using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetExistingCartItem : IGetExistingCartItem
    {
        private readonly ISession _session;

        public GetExistingCartItem(ISession session)
        {
            _session = session;
        }

        public CartItem GetExistingItem(CartModel cart, ProductVariant item, string data)
        {
            var itemData = cart.Items.FirstOrDefault(
                cartItem => cartItem.Item.Id == item.Id && cartItem.Data == data);

            return itemData == null
                ? null
                : _session.Get<CartItem>(itemData.Id);
        }
    }
}