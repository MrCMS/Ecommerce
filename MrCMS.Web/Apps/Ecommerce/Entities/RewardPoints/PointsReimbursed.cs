namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public class PointsReimbursed : OrderRewardPointsHistory
    {
        public virtual PointsSpent ReimbursmentFor { get; set; }
        public override string DisplayName
        {
            get { return string.Format("{0} points reimbursed for order #{1}", Points, Order.Id); }
        }
    }
}