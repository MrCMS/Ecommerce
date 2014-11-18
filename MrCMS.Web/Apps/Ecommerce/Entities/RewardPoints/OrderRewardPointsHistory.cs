using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public abstract class OrderRewardPointsHistory : RewardPointsHistory
    {
        public virtual Order Order { get; set; }
    }
}