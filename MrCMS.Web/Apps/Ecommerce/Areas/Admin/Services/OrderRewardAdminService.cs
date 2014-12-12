using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class OrderRewardAdminService : IOrderRewardAdminService
    {
        private readonly ISession _session;

        public OrderRewardAdminService(ISession session)
        {
            _session = session;
        }

        public IList<OrderRewardPointsHistory> GetOrderRewardPointsUsage(Order order)
        {
            return _session.QueryOver<OrderRewardPointsHistory>()
                .Where(history => history.Order.Id == order.Id)
                .OrderBy(history => history.CreatedOn).Asc
                .Cacheable().List();
        }
    }
}