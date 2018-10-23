using System;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services.Auth;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class CopyCartToUser : IOnUserLoggedIn, IOnUserRegistered
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly IGetExistingCartItem _getExistingCartItem;
        private readonly ISession _session;

        public CopyCartToUser(ISession session, ICartBuilder cartBuilder, IGetExistingCartItem getExistingCartItem)
        {
            _session = session;
            _cartBuilder = cartBuilder;
            _getExistingCartItem = getExistingCartItem;
        }

        public void Execute(UserLoggedInEventArgs args)
        {
            UserLoggedIn(args.User, args.PreviousSession);
        }

        public void Execute(OnUserRegisteredEventArgs args)
        {
            UserLoggedIn(args.User, args.PreviousSession);
        }

        private void UserLoggedIn(User user, Guid previousSession)
        {
            var itemsToCopy = _session.QueryOver<CartItem>().Where(item => item.UserGuid == previousSession).List();

            var cart = _cartBuilder.BuildCart();
            _session.Transact(session =>
            {
                foreach (var item in itemsToCopy)
                {
                    var cartItem = _getExistingCartItem.GetExistingItem(cart, item.Item, item.Data);
                    if (cartItem != null)
                    {
                        cartItem.Quantity += item.Quantity;
                    }
                    else
                    {
                        cartItem = new CartItem
                        {
                            Item = item.Item,
                            Quantity = item.Quantity,
                            UserGuid = CurrentRequestData.UserGuid
                        };
                        cart.Items.Add(cartItem.GetCartItemData());
                    }
                    session.SaveOrUpdate(cartItem);
                    session.Delete(item);
                }
            });
        }
    }
}