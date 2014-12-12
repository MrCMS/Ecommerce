namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public class PointsRemoved : OrderRewardPointsHistory
    {
        public virtual PointsAwarded RemovalFor { get; set; }
        public override string DisplayName
        {
            get { return string.Format("{0} points removed for order #{1}", -Points, Order.Id); }
        }
    }
}