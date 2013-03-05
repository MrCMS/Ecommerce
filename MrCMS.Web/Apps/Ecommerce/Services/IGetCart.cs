using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IGetCart
    {
        CartModel GetCart();
    }

    public class GetCartImpl : IGetCart
    {
        private readonly ISession _session;

        public GetCartImpl(ISession session)
        {
            _session = session;
        }

        public CartModel GetCart()
        {
            return new CartModel
                       {
                           User = CurrentRequestData.CurrentUser,
                           UserGuid = CurrentRequestData.UserGuid,
                           Items = GetItems()
                       };
        }

        private List<CartItem> GetItems()
        {
            return
                _session.QueryOver<CartItem>()
                        .Where(item => item.UserGuid == CurrentRequestData.UserGuid)
                        .Cacheable()
                        .List().ToList();
        }
    }
}