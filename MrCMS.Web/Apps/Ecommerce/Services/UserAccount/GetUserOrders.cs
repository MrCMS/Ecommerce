using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public class GetUserOrders : IGetUserOrders
    {
        private readonly ISession _session;

        public GetUserOrders(ISession session)
        {
            _session = session;
        }

        public IPagedList<Order> Get(User user, int page = 1)
        {
            return
                _session.QueryOver<Order>()
                    .Where(x => x.User.Id == user.Id)
                    .OrderBy(x => x.Id)
                    .Desc.Cacheable()
                    .List()
                    .ToPagedList(page);
        }
    }
}