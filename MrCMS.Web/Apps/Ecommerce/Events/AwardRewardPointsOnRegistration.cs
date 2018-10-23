using MrCMS.Helpers;
using MrCMS.Services.Auth;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class AwardRewardPointsOnRegistration : IOnUserRegistered
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly RewardPointSettings _rewardPointSettings;
        private readonly ISession _session;

        public AwardRewardPointsOnRegistration(EcommerceSettings ecommerceSettings,
            RewardPointSettings rewardPointSettings, ISession session)
        {
            _ecommerceSettings = ecommerceSettings;
            _rewardPointSettings = rewardPointSettings;
            _session = session;
        }

        public void Execute(OnUserRegisteredEventArgs args)
        {
            if (!_ecommerceSettings.RewardPointsEnabled)
                return;

            var pointsForRegistration = _rewardPointSettings.PointsForRegistration;
            if (pointsForRegistration <= 0)
                return;
            var user = args.User;
            var registrationPoints = new RegistrationPoints
            {
                User = user,
                Points = pointsForRegistration
            };
            _session.Transact(session => session.Save(registrationPoints));
        }
    }
}