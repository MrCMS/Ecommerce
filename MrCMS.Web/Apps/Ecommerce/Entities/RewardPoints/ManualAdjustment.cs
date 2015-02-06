using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public class ManualAdjustment : RewardPointsHistory
    {
        public virtual User AdjustedBy { get; set; }

        public override string DisplayName
        {
            get
            {
                return string.Format("Manual adjustment of {0} points for {1} by {2}", Points, User.Name,
                    AdjustedBy.Name);
            }
        }
    }
}