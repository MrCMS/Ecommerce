namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public class PointsAwarded : OrderRewardPointsHistory
    {
        public override string DisplayName
        {
            get { return string.Format("{0} points awarded for order #{1}", Points, Order.Id); }
        }
    }
}