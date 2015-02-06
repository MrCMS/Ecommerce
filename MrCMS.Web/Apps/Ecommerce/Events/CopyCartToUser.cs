using System;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class CopyCartToUser : IOnUserLoggedIn, IOnUserRegistered
    {
        private readonly ISession _session;
        private readonly ICartBuilder _cartBuilder;

        public CopyCartToUser(ISession session, ICartBuilder cartBuilder)
        {
            _session = session;
            _cartBuilder = cartBuilder;
        }

        private void UserLoggedIn(User user, Guid previousSession)
        {
            var itemsToCopy = _session.QueryOver<CartItem>().Where(item => item.UserGuid == previousSession).List();

            var cart = _cartBuilder.BuildCart();
            _session.Transact(session =>
            {
                foreach (var item in itemsToCopy)
                {
                    var existingItem =
                        cart.Items.FirstOrDefault(
                            cartItem => cartItem.Item.SKU == item.Item.SKU);
                    if (existingItem != null)
                        existingItem.Quantity += item.Quantity;
                    else
                    {
                        existingItem = new CartItem
                        {
                            Item = item.Item,
                            Quantity = item.Quantity,
                            UserGuid = CurrentRequestData.UserGuid
                        };
                        cart.Items.Add(existingItem);
                    }
                    session.SaveOrUpdate(existingItem);
                    session.Delete(item);
                }

            });
        }

        public void Execute(UserLoggedInEventArgs args)
        {
            UserLoggedIn(args.User, args.PreviousSession);
        }

        public void Execute(OnUserRegisteredEventArgs args)
        {
            UserLoggedIn(args.User, args.PreviousSession);
        }
    }
}