namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public class PointsSpent : OrderRewardPointsHistory
    {
        public override string DisplayName
        {
            get { return string.Format("{0} points spent on order #{1}", -Points, Order.Id); }
        }
    }
}