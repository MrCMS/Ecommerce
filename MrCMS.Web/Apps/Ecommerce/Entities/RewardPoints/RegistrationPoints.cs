namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public class RegistrationPoints : RewardPointsHistory 
    {
        public override string DisplayName
        {
            get { return string.Format("{0} points awarded to {1} for registering", Points, User.Name); }
        }
    }
}