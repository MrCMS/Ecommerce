using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IOrderRewardAdminService
    {
        IList<OrderRewardPointsHistory> GetOrderRewardPointsUsage(Order order);
    }
}